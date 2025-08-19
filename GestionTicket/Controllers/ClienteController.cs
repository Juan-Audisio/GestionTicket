using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GestionTicket.Models;
using System.Security.Claims;

namespace GestionTicket.Controllers

{
    [Route("api/[controller]")]
    [ApiController]

    public class ClienteController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClienteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [Authorize(Roles = "ADMINISTRADOR")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes([FromQuery] Cliente cliente)
    {
        var clientes = _context.Clientes.AsQueryable();
    

            clientes = clientes.OrderBy(c => c.Nombre);

        return await clientes.ToListAsync();
    }
    [Authorize(Roles = "ADMINISTRADOR")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
            return NotFound();
        return Ok(cliente);
    }

    [Authorize(Roles = "ADMINISTRADOR")]
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Cliente cliente)
    {
        if (!string.IsNullOrEmpty(cliente.Nombre) &&
            !string.IsNullOrEmpty(cliente.DNI) &&
            !string.IsNullOrEmpty(cliente.Email))
        {
            if (!_context.Clientes.Any(c => c.DNI == cliente.DNI || c.Email == cliente.Email))
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                var userId = new ApplicationUser
                {
                    UserName = cliente.Email,
                    Email = cliente.Email,
                    NombreCompleto = cliente.Nombre
                };

                var result = await _userManager.CreateAsync(userId, "Ezpeleta2025");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userId, "CLIENTE");
                }

                return CreatedAtAction("GetCliente", new { id = cliente.ClienteID }, cliente);
            }
            else
            {
                return BadRequest("Ya existe un cliente con ese DNI o Email");
            }
        }

        return BadRequest("Faltan campos obligatorios");
   


        // var usuarioLogueadoId = HttpContext.User.Identity.Name;
        // var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // var rol = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

        //     if (string.IsNullOrEmpty(userId))
        //     {
        //         return Unauthorized("No se pudo identificar al usuario.");
        //     }
        //     cliente.UsuarioClienteID = userId; // Asegurar que se establece el usuario
        

            
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(ModelState);
        // }
        //     var existeCliente = await _context.Clientes
        //         .AnyAsync(c => c.Nombre.ToLower() == cliente.Nombre.ToLower() || c.DNI == cliente.DNI);

        // if (existeCliente)
        // {
        //    return BadRequest("Ya existe un cliente con ese nombre o DNI"); 
        // }

        // _context.Clientes.Add(cliente);
        // await _context.SaveChangesAsync();
        //        return CreatedAtAction(nameof(Obtener), new { id = cliente.ClienteID }, cliente);
        }
    
        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] Cliente cliente)
        {
            
            var existente = await _context.Clientes.FindAsync(id);
            if (existente == null)
                return NotFound();
                
            var existeCliente = await _context.Clientes
                .AnyAsync(c => c.ClienteID != id && 
                    (c.Nombre.ToLower() == cliente.Nombre.ToLower() || 
                        c.DNI == cliente.DNI));
            if (existeCliente)
            {
            return BadRequest("Ya existe otro cliente con ese nombre o DNI"); 
            }
            
            existente.Nombre = cliente.Nombre;
            existente.DNI = cliente.DNI;
            existente.Email = cliente.Email;
            existente.Telefono = cliente.Telefono;
            existente.Observaciones = cliente.Observaciones;

            await _context.SaveChangesAsync();
            return Ok(existente);
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPut("estado/{id}")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] Cliente estadoCliente)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            cliente.Eliminado = estadoCliente.Eliminado;
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }


        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

}
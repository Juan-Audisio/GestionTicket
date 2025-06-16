using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GestionTicket.Models;
using System.Security.Claims;

namespace GestionTicket.Controllers

{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class ClienteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClienteController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes([FromQuery] Cliente cliente)
    {
        var clientes = _context.Clientes.AsQueryable();
    

            clientes = clientes.OrderBy(c => c.Nombre);

        return await clientes.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
            return NotFound();
        return Ok(cliente);
    }


    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Cliente cliente)
    {
        var usuarioLogueadoId = HttpContext.User.Identity.Name;
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var rol = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }
            cliente.UsuarioClienteID = userId; // Asegurar que se establece el usuario
        

            
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
            var existeCliente = await _context.Clientes
                .AnyAsync(c => c.Nombre.ToLower() == cliente.Nombre.ToLower() || 
                   c.DNI == cliente.DNI);

        if (existeCliente)
        {
           return BadRequest("Ya existe un cliente con ese nombre o DNI"); 
        }

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return CreatedAtAction("Obtener", new { id = cliente.TicketID }, cliente);
    }

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
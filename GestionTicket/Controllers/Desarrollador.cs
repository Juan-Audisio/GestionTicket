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

    public class DesarrolladorController : ControllerBase 
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public DesarrolladorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Desarrollador>>> GetDesarrollador([FromQuery] Desarrollador desarrollador)
        {
            var desarrolladores = _context.Desarrolladores.AsQueryable();
        

                desarrolladores = desarrolladores
                .Include(d => d.PuestoLaborales)
                .OrderBy(d => d.Nombre);

            return await desarrolladores.ToListAsync();
        }
        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var desarrollador = await _context.Desarrolladores.FindAsync(id);
            if (desarrollador == null)
                return NotFound();
            return Ok(desarrollador);
        }
        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Desarrollador desarrollador)
        {
            if (!string.IsNullOrEmpty(desarrollador.Nombre) &&
                !string.IsNullOrEmpty(desarrollador.DNI) &&
                !string.IsNullOrEmpty(desarrollador.Email))
            {
                if (!_context.Desarrolladores.Any(c => c.DNI == desarrollador.DNI || c.Email == desarrollador.Email))
                {

                    var userId = new ApplicationUser
                    {
                        UserName = desarrollador.Email,
                        Email = desarrollador.Email,
                        NombreCompleto = desarrollador.Nombre
                    };

                    var result = await _userManager.CreateAsync(userId, "Ezpeleta2025");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(userId, "DESARROLLADOR");
                        // Asignar el ID del desarrollador al usuario
                        desarrollador.UsuarioClienteID = userId.Id;
                    }
                    _context.Desarrolladores.Add(desarrollador);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetDesarrollador", new { id = desarrollador.DesarrolladorID }, desarrollador);
                }
                else
                {
                    return BadRequest("Ya existe un Desarrollador con ese DNI o Email");
                }
            }

            return BadRequest("Faltan campos obligatorios");
        }
        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody]  Desarrollador desarrollador)
        {
            
            var existente = await _context.Desarrolladores.FindAsync(id);
            if (existente == null)
                return NotFound();
                
            var existeDesarrollador = await _context.Desarrolladores
                .AnyAsync(c => c.DesarrolladorID != id && 
                    (c.Nombre.ToLower() == desarrollador.Nombre.ToLower() || 
                        c.DNI == desarrollador.DNI));
            if (existeDesarrollador)
            {
            return BadRequest("Ya existe otro desarrollador con ese nombre o DNI"); 
            }
            
            existente.Nombre = desarrollador.Nombre;
            existente.DNI = desarrollador.DNI;
            existente.Email = desarrollador.Email;
            existente.Telefono = desarrollador.Telefono;
            existente.Observaciones = desarrollador.Observaciones;

            await _context.SaveChangesAsync();
            return Ok(existente);
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPut("estado/{id}")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] Desarrollador estadoDesarrollador)
        {
            var desarrollador = await _context.Desarrolladores.FindAsync(id);
            if (desarrollador == null)
                return NotFound();

            desarrollador.Eliminado = estadoDesarrollador.Eliminado;
            await _context.SaveChangesAsync();

            return Ok(desarrollador);
        }
        
    }
} 
    

    
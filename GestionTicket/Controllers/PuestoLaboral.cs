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

    public class PuestoLaboralController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PuestoLaboralController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }


            // [Authorize(Roles = "ADMINISTRADOR")]
            [HttpGet]
            public async Task<ActionResult<IEnumerable<PuestoLaboral>>> GetPuestoLaborales([FromQuery] bool? eliminado)
            {
                var puestoLaborales = _context.PuestoLaborales.AsQueryable();
            
                    if (eliminado.HasValue)
                    {
                        puestoLaborales = puestoLaborales.Where(c => c.Eliminado == eliminado.Value);
                    }

                return await puestoLaborales.ToListAsync();
            }



            // [Authorize(Roles = "ADMINISTRADOR")]
            [HttpGet("{id}")]
            public async Task<IActionResult> Obtener(int id)
            {
                var puestoLaboral = await _context.PuestoLaborales.FindAsync(id);
                if (puestoLaboral == null)
                    return NotFound();
                return Ok(puestoLaboral);
            }




            [Authorize(Roles = "ADMINISTRADOR")]
            [HttpPost]
            public async Task<IActionResult> Crear([FromBody] PuestoLaboral puestoLaboral)
            {   
                var usuarioLogueadoId = HttpContext.User.Identity.Name;
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var rol = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

                    if (string.IsNullOrEmpty(userId))
                    {
                        return Unauthorized("No se pudo identificar al usuario.");
                    }
                    puestoLaboral.UsuarioClienteID = userId;

                var existePuestolaborales = await _context.PuestoLaborales
                    .AnyAsync(p => p.Descripcion.ToLower() == puestoLaboral.Descripcion.ToLower());
                    
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.PuestoLaborales.Add(puestoLaboral);
                await _context.SaveChangesAsync();
                return Ok(puestoLaboral);
            }

            

            [Authorize(Roles = "ADMINISTRADOR")]
            [HttpPut("{id}")]
            public async Task<IActionResult> Editar(int id, [FromBody] PuestoLaboral puestoLaboral)
            {
                var existePuestoLaboral = await _context.PuestoLaborales
                    .AnyAsync(c => c.Descripcion.ToLower() == puestoLaboral.Descripcion.ToLower() && c.PuestoLaboralID != id);

                var existente = await _context.PuestoLaborales.FindAsync(id);
                if (existente == null)
                    return NotFound();
                

                existente.Descripcion = puestoLaboral.Descripcion;

                

                await _context.SaveChangesAsync();
                return Ok(existente);
            }
            [Authorize(Roles = "ADMINISTRADOR")]
            [HttpPut("estado/{id}")]
            public async Task<IActionResult> CambiarEstado(int id, [FromBody] PuestoLaboral estadoPuestoLaboral)
            {
                var puestoLaboral = await _context.PuestoLaborales.FindAsync(id);
                if (puestoLaboral == null)
                    return NotFound();

                puestoLaboral.Eliminado = estadoPuestoLaboral.Eliminado;
                await _context.SaveChangesAsync();

                return Ok(puestoLaboral);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> puestoCategoria(int id, [FromBody] PuestoLaboral puestoLaboral)
            {
                var existePuestoLaboral = await _context.PuestoLaborales
                    .AnyAsync(c => c.Descripcion.ToLower() == puestoLaboral.Descripcion.ToLower() && c.PuestoLaboralID != id);

                var existente = await _context.PuestoLaborales.FindAsync(id);
                if (existente == null)
                    return NotFound();
                

                existente.Descripcion = puestoLaboral.Descripcion;

                

                await _context.SaveChangesAsync();
                return Ok(existente);
            }
            


    }
  
}
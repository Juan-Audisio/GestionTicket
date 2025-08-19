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

    public class PuestoCategoriaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PuestoCategoriaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PuestoCategoria>>> GetPuestoCategorias([FromQuery] bool? eliminado)
        {
            var puestoCategorias = _context.PuestoCategorias
            .Include(pc => pc.PuestoLaborales) // Incluir los puestos laborales relacionados
            .Include(c => c.Categorias) // Incluir la categoría relacionada
            .ToListAsync();

            return Ok(puestoCategorias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var puestoCategoria = await _context.PuestoCategorias.FindAsync(id);
            if (puestoCategoria == null)
                return NotFound();
            return Ok(puestoCategoria);
        }

            [Authorize(Roles = "ADMINISTRADOR")]
            [HttpPost]
            public async Task<IActionResult> Crear([FromBody] PuestoCategoria puestoCategoria)
            {
            if (puestoCategoria == null)
            {
                return BadRequest("El puesto de categoría no puede ser nulo.");
            }

            _context.PuestoCategorias.Add(puestoCategoria);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Obtener), new { id = puestoCategoria.PuestoCategoriaID }, puestoCategoria);
        }
    }
}
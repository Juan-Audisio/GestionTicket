using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using GestionTicket.Models;

namespace GestionTicket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

[HttpGet]
public async Task<ActionResult<IEnumerable<TicketVista>>> GetTickets()
{
    var tickets = await _context.Tickets
        .Include(t => t.Categorias)
        .Select(t => new TicketVista {
            TicketID = t.TicketID,
            Titulo = t.Titulo,
            Descripcion = t.Descripcion,
            Estado = t.Estado.ToString(),
            Prioridad = t.Prioridad.ToString(),
            FechaCreacion = t.FechaCreacion,
            // FechaCreacionString = t.FechaCreacion.ToString("dd/MM/yyyy"),
            FechaCierre = t.FechaCierre,
            UsuarioClienteID = t.UsuarioClienteID,
            CategoriaID = t.CategoriaID,
            CategoriaDescripcion = t.Categorias.Descripcion.ToString(),
        })
        .ToListAsync();

    return Ok(tickets);
}

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();
            return Ok(ticket);
        }

        [HttpGet("estados")]
        public IActionResult ObtenerEstados()
        {
            var lista = new List<object>
            {
                new { id = 0, nombre = "-Seleccione Estado-" }
            };

            lista.AddRange(Enum.GetValues(typeof(Estado))
                .Cast<Estado>()
                .Select(e => new {
                    id = (int)e,
                    nombre = e.ToString().ToUpper()
                }));

            return Ok(lista);
        }

        [HttpGet("prioridades")]
        public IActionResult ObtenerPrioridades()
        {
            var lista = new List<object>
            {
                new { id = 0, nombre = "-Seleccione Prioridad-" }
            };

            lista.AddRange(Enum.GetValues(typeof(Prioridad))
                .Cast<Prioridad>()
                .Select(p => new {
                    id = (int)p,
                    nombre = p.ToString().ToUpper()
                }));

            return Ok(lista);
        }

        [HttpGet("listado")]
        public async Task<IActionResult> ObtenerCategorias()
        {
            var categorias = await _context.Categorias
                // .Where(c => !c.Eliminado) // O quitar esta línea si no usás "Eliminado"
                .Select(c => new {
                    id = c.CategoriaID,
                    nombre = c.Descripcion
                })
                .ToListAsync();

            return Ok(categorias);
        }

      [HttpPost]
public async Task<ActionResult<Ticket>> PostTicket(Ticket ticket)
{
    if (ticket == null)
    {
        return BadRequest("No se pudo procesar el ticket.");
    }

    // Asignamos la fecha de creación automáticamente
    ticket.FechaCreacion = DateTime.Now;

    // Añadimos el ticket a la base de datos
    _context.Tickets.Add(ticket);

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        return BadRequest($"Error al guardar el ticket: {ex.Message}");
    }

    return CreatedAtAction("GetTicket", new { id = ticket.TicketID }, ticket);
}

    }
}


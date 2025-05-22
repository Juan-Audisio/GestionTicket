using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using GestionTicket.Models;
using Microsoft.AspNetCore.Authorization;

namespace GestionTicket.Controllers
{
    [Authorize]
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
                .Select(t => new TicketVista
                {
                    TicketID = t.TicketID,
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    Estado = t.Estado.ToString(),
                    Prioridad = t.Prioridad.ToString(),
                    FechaCreacion = t.FechaCreacion,
                    // FechaCreacionString = t.FechaCreacion.ToString("dd/MM/yyyy"),
                    FechaCierre = t.FechaCierre,
                    // UsuarioClienteID = t.UsuarioClienteID,
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

        [HttpPost]
        public async Task<ActionResult<Ticket>> PostTicket(Ticket ticket)
        {
            if (ticket == null)
            {
                return BadRequest("No se pudo procesar el ticket.");
            }

            // Asignamos la fecha de creación automáticamente
            ticket.FechaCreacion = DateTime.Now;
            ticket.FechaCierre = Convert.ToDateTime("01/01/2025");
            ticket.Estado = Estado.Abierto;
            // Añadimos el ticket a la base de datos
            _context.Tickets.Add(ticket);

            await _context.SaveChangesAsync();

            return Ok(ticket);
        }

[HttpPut("{id}")]
public async Task<IActionResult> EditarTicket(int id, Ticket ticket)
{
    if (id != ticket.TicketID)
    {
        return BadRequest("ID del ticket no coincide");
    }

    
    var original = await _context.Tickets
        .AsNoTracking()
        .FirstOrDefaultAsync(t => t.TicketID == id);
    
    if (original == null)
    {
        return NotFound();
    }

    if (original.Titulo != ticket.Titulo)
    {
        var cambiotitulo = new HistorialTicket
        {
            TicketID = id,
            CampoModificado = "TITULO",
            ValorAnterior = original.Titulo,
            ValorNuevo = ticket.Titulo,
            FechaCambio = DateTime.Now
        };
        _context.HistorialTicket.Add(cambiotitulo);
    }

    if (original.Descripcion != ticket.Descripcion)
    {
        var cambiodescripcion = new HistorialTicket
        {
            TicketID = id,
            CampoModificado = "DESCRIPCION",
            ValorAnterior = original.Descripcion,
            ValorNuevo = ticket.Descripcion,
            FechaCambio = DateTime.Now
        };
        _context.HistorialTicket.Add(cambiodescripcion);
    }

    if (original.CategoriaID != ticket.CategoriaID)
    {
        var cambiocategoria = new HistorialTicket
        {
            TicketID = id,
            CampoModificado = "CATEGORIA",
            ValorAnterior = original.CategoriaID.ToString(),
            ValorNuevo = ticket.CategoriaID.ToString(),
            FechaCambio = DateTime.Now
        };
        _context.HistorialTicket.Add(cambiocategoria);
    }

    if (original.Prioridad != ticket.Prioridad)
    {
        var cambioprioridad = new HistorialTicket
        {
            TicketID = id,
            CampoModificado = "PRIORIDAD",
            ValorAnterior = original.Prioridad.ToString(),
            ValorNuevo = ticket.Prioridad.ToString(),
            FechaCambio = DateTime.Now
        };
        _context.HistorialTicket.Add(cambioprioridad);
    }

    // Ahora sí puedes marcar como Modified porque original no está siendo rastreada
    _context.Entry(ticket).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!TicketExists(id))
        {
            return NotFound();
        }
        else
        {
            throw;
        }
    }

    return NoContent();
}

        // Método auxiliar para verificar si existe un ticket - COLOCADO DENTRO DE LA CLASE
        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketID == id);
        }
    }
}
// todo se hace en la funcion editar
//en la funcion editar ticket capturar al momento de hacer un modificar y esa captura guardarla para luego mostrarla en una lista.
//necesitamos ver verificar que campo lo cambio.
//necesito preguntar si el registro cambio, y si cambio registrar ese cambio para luego mostrarlo en la tabla

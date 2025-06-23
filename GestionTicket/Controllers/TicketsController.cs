using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using GestionTicket.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

             // Obtenemos el ID del usuario autenticado desde el token JWT
            var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }

            var tickets = await _context.Tickets
                .Where(t => t.UsuarioClienteID == userId)
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
            var usuarioLogueadoId = HttpContext.User.Identity.Name;
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }
            // Seteo de valores por defecto
            ticket.Estado = Estado.Abierto;
            ticket.FechaCreacion = DateTime.Now;
            ticket.UsuarioClienteID = userId; // Asegurar que se establece el usuario

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Obtener", new { id = ticket.TicketID }, ticket);
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



       [HttpPost("filtrar")]
        public async Task<ActionResult<IEnumerable<TicketVista>>> FiltrarTickets([FromBody] TicketFiltroDTO filtro)
        {
            try
            {
                Console.WriteLine($"Filtro recibido - CategoriaID: {filtro?.CategoriaID}");
                if (filtro == null)
                {
                    Console.WriteLine("Filtro es null");
                    return BadRequest("Filtro no puede ser null");
                }


                var vista = new List<TicketVista>();
                var tickets = _context.Tickets.Include(t => t.Categorias).AsQueryable();

                Console.WriteLine($"Total tickets antes del filtro: {await tickets.CountAsync()}");

                if (filtro.CategoriaID > 0)
                    tickets = tickets.Where(t => t.CategoriaID == filtro.CategoriaID);
                if (filtro.Prioridad > 0)
                    tickets = tickets.Where(t => t.Prioridad == (Prioridad)filtro.Prioridad);
                    
                if (filtro.Estado > 0)
                    tickets = tickets.Where(t => t.Estado == (Estado)filtro.Estado);

                if (filtro.FechaDesde.HasValue && filtro.FechaHasta.HasValue)
                {
                    if (filtro.FechaDesde > filtro.FechaHasta)
                        return BadRequest("La fecha 'Desde' no puede ser mayor que la fecha 'Hasta'");

                    tickets = tickets.Where(t =>
                        t.FechaCreacion >= filtro.FechaDesde.Value &&
                        t.FechaCreacion <= filtro.FechaHasta.Value);
                }
                else if (filtro.FechaDesde.HasValue)
                {
                    tickets = tickets.Where(t => t.FechaCreacion >= filtro.FechaDesde.Value);
                }
                else if (filtro.FechaHasta.HasValue)
                {
                    tickets = tickets.Where(t => t.FechaCreacion <= filtro.FechaHasta.Value);
                }


                foreach (var ticket in tickets.OrderByDescending(t => t.FechaCreacion))
                {
                    vista.Add(new TicketVista
                    {
                        TicketID = ticket.TicketID,
                        Titulo = ticket.Titulo,
                        Descripcion = ticket.Descripcion,
                        Estado = ticket.Estado.ToString(),
                        Prioridad = ticket.Prioridad.ToString(),
                        FechaCreacion = ticket.FechaCreacion,
                        FechaCierre = ticket.FechaCierre,
                        CategoriaID = ticket.CategoriaID,
                        CategoriaDescripcion = ticket.Categorias != null ? ticket.Categorias.Descripcion : "Sin categoría"
                    });
                }

                return vista;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error completo en FiltrarTickets: {ex}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}

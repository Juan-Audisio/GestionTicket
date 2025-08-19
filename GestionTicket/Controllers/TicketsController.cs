using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GestionTicket.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [Authorize(Roles = "ADMINISTRADOR,CLIENTE,DESARROLLADOR")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketVista>>> GetTickets()
        {
            // Obtener información del usuario logueado
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("No se pudo identificar al usuario.");

            // Traer los tickets base
            var query = _context.Tickets
                                .Include(t => t.Categorias)
                                .AsQueryable();

            // Filtrar según rol
            if (!string.Equals(userRole, "ADMINISTRADOR", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(userRole, "CLIENTE", StringComparison.OrdinalIgnoreCase))
                {
                    // Cliente ve solo sus tickets
                    query = query.Where(t => t.UsuarioClienteID == userId);
                }
                else if (string.Equals(userRole, "DESARROLLADOR", StringComparison.OrdinalIgnoreCase))
                {
                    // Desarrollador ve tickets según categorías de su puesto
                    var desarrollador = await _context.Desarrolladores
                                                    .Include(d => d.PuestoLaborales)
                                                    .FirstOrDefaultAsync(d => d.UsuarioClienteID == userId);

                    if (desarrollador != null && desarrollador.PuestoLaboralID.HasValue)
                    {
                        var categoriaIds = await _context.PuestoCategorias
                                                        .Where(pc => pc.PuestoLaboralID == desarrollador.PuestoLaboralID.Value)
                                                        .Select(pc => pc.CategoriaID)
                                                        .ToListAsync();

                        query = query.Where(t => categoriaIds.Contains(t.CategoriaID));
                    }
                    else
                    {
                        // Si no tiene puesto asignado, no ve ningún ticket
                        query = query.Where(t => false);
                    }
                }
            }

            // Ejecutar query y proyectar a vista
            var tickets = await query
                .OrderBy(t => t.FechaCreacion)
                .Select(t => new TicketVista
                {
                    TicketID = t.TicketID,
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    Estado = t.Estado.ToString(),
                    Prioridad = t.Prioridad.ToString(),
                    FechaCreacion = t.FechaCreacion,
                    FechaCierre = t.FechaCierre,
                    CategoriaID = t.CategoriaID,
                    CategoriaDescripcion = t.Categorias.Descripcion
                })
                .ToListAsync();

            return Ok(tickets);
        }
        [Authorize(Roles = "ADMINISTRADOR,CLIENTE,DESARROLLADOR")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
             var ticket = await _context.Tickets
                .Include(t => t.UsuarioCliente)
                .FirstOrDefaultAsync(t => t.TicketID == id);

            if (ticket == null)
                return NotFound();

            return Ok(new
            {
                ticket.TicketID,
                ticket.Titulo,
                ticket.Descripcion,
                ticket.CategoriaID,
                ticket.Prioridad,
                ticket.Estado,
                ticket.FechaCreacion,
                ticket.FechaCierre,
                Email = ticket.UsuarioCliente?.Email
                });
                }

        [Authorize(Roles = "ADMINISTRADOR,CLIENTE,DESARROLLADOR")]
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
            ticket.UsuarioClienteID = userId;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Obtener", new { id = ticket.TicketID }, ticket);
        }

        [Authorize(Roles = "ADMINISTRADOR,CLIENTE,DESARROLLADOR")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarTicket(int id, Ticket ticket)
        {
            var emailUsuario = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

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
                    FechaCambio = DateTime.Now,
                    UsuarioEmail = emailUsuario
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
                    FechaCambio = DateTime.Now,
                    UsuarioEmail = emailUsuario
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
                    FechaCambio = DateTime.Now,
                    UsuarioEmail = emailUsuario
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
                    FechaCambio = DateTime.Now,
                    UsuarioEmail = emailUsuario
                };
                _context.HistorialTicket.Add(cambioprioridad);
            }

            ticket.Estado = Estado.Abierto;
            ticket.FechaCreacion = DateTime.Now;

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


       [Authorize(Roles = "ADMINISTRADOR,CLIENTE,DESARROLLADOR")]
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

                var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("No se pudo identificar al usuario.");
                    
                tickets = tickets.Where(t => t.UsuarioClienteID == userId);

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

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
    public class visualisarclientecontroller : ControllerBase
    {
        private readonly ApplicationDbContext _context;

    //     // public visualisarClientesController(ApplicationDbContext context)
    //     // {
    //     //     _context = context;
    //     // }

    //     [Authorize(Roles = "ADMINISTRADOR")]
    //     [HttpPost("filtrar")]
    //     public async Task<ActionResult<IEnumerable<TicketVista>>> FiltrarTickets([FromBody] TicketFiltroDTO filtro)
    //     {
    //         try
    //         {
    //             Console.WriteLine($"Filtro recibido - CategoriaID: {filtro?.CategoriaID}");
    //             if (filtro == null)
    //             {
    //                 Console.WriteLine("Filtro es null");
    //                 return BadRequest("Filtro no puede ser null");
    //             }


    //             var vista = new List<TicketVista>();
    //             var tickets = _context.Tickets.Include(t => t.Clientes).AsQueryable();

    //             var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    //             if (string.IsNullOrEmpty(userId))
    //                 return Unauthorized("No se pudo identificar al usuario.");
                    
    //             tickets = tickets.Where(t => t.UsuarioClienteID == userId);

    //             Console.WriteLine($"Total tickets antes del filtro: {await tickets.CountAsync()}");

    //             if (filtro.ClienteID > 0)
    //                 tickets = tickets.Where(t => t.ClienteID == filtro.ClienteID);
    //             if (filtro.FechaDesde.HasValue && filtro.FechaHasta.HasValue)
    //             {
    //                 if (filtro.FechaDesde > filtro.FechaHasta)
    //                     return BadRequest("La fecha 'Desde' no puede ser mayor que la fecha 'Hasta'");

    //                 tickets = tickets.Where(t =>
    //                     t.FechaCreacion >= filtro.FechaDesde.Value &&
    //                     t.FechaCreacion <= filtro.FechaHasta.Value);
    //             }
    //             else if (filtro.FechaDesde.HasValue)
    //             {
    //                 tickets = tickets.Where(t => t.FechaCreacion >= filtro.FechaDesde.Value);
    //             }
    //             else if (filtro.FechaHasta.HasValue)
    //             {
    //                 tickets = tickets.Where(t => t.FechaCreacion <= filtro.FechaHasta.Value);
    //             }


    //             foreach (var ticket in tickets.OrderByDescending(t => t.FechaCreacion))
    //             {
    //                 vista.Add(new TicketVista
    //                 {
    //                     TicketID = ticket.TicketID,
    //                     Titulo = ticket.Titulo,
    //                     Descripcion = ticket.Descripcion,
    //                     Estado = ticket.Estado.ToString(),
    //                     Prioridad = ticket.Prioridad.ToString(),
    //                     FechaCreacion = ticket.FechaCreacion,
    //                     FechaCierre = ticket.FechaCierre,
    //                     CategoriaID = ticket.CategoriaID,
    //                     CategoriaDescripcion = ticket.Categorias != null ? ticket.Categorias.Descripcion : "Sin categoría"
    //                 });
    //             }

    //             return vista;
    //         }
    //         catch (Exception ex)
    //         {
    //             return StatusCode(500, $"Error interno: {ex.Message}");
    //         }
    //     }
    }
}
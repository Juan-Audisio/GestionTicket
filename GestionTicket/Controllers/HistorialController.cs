using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionTicket.Models;
using Microsoft.AspNetCore.Authorization;

namespace GestionTicket.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HistorialController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Historial      
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<HistorialTicket>>> GetHistorial(int id)
        {
            return await _context.HistorialTicket.Where(h => h.TicketID == id).OrderByDescending(c => c.FechaCambio).ToListAsync();
        }        
    }
}
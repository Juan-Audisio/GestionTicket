using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using GestionTicket.Models;

namespace GestionTicket.Controllers
{

[Route("api/[controller]")]
[ApiController]
public class CategoriaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias([FromQuery] bool? eliminado)
    {
        var categorias = _context.Categorias.AsQueryable();

        if (eliminado.HasValue)
        {
            categorias = categorias.Where(c => c.Eliminado == eliminado.Value);
        }

        return await categorias.ToListAsync();
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
            return NotFound();
        return Ok(categoria);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Categoria categoria)
    {

        var existeCategoria = await _context.Categorias
            .AnyAsync(c => c.Descripcion.ToLower() == categoria.Descripcion.ToLower());
            
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return Ok(categoria);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] Categoria categoria)
    {
        var existeCategoria = await _context.Categorias
            .AnyAsync(c => c.Descripcion.ToLower() == categoria.Descripcion.ToLower() && c.CategoriaID != id);

        var existente = await _context.Categorias.FindAsync(id);
        if (existente == null)
            return NotFound();
          

        existente.Descripcion = categoria.Descripcion;
        // existente.Eliminado = categoria.Eliminado;

        

        await _context.SaveChangesAsync();
        return Ok(existente);
    }

    [HttpPut("estado/{id}")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] Categoria estadoCategoria)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
            return NotFound();

        categoria.Eliminado = estadoCategoria.Eliminado;
        await _context.SaveChangesAsync();

        return Ok(categoria);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
            return NotFound();

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return Ok();
    }

    }

}

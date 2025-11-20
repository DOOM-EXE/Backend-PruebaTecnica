using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VentasApiPrueba.Models.DTOs;
using VentasApiPrueba.Services.Interfaces;

namespace VentasApiPrueba.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    /// Obtiene todas las categorías
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll()
    {
        var categorias = await _categoriaService.GetAllAsync();
        return Ok(categorias);
    }

    /// Obtiene una categoría por ID
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoriaDto>> GetById(int id)
    {
        var categoria = await _categoriaService.GetByIdAsync(id);
        
        if (categoria == null)
            return NotFound(new { message = $"Categoría con ID {id} no encontrada" });
        
        return Ok(categoria);
    }

    /// Crea una nueva categoría
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoriaDto>> Create([FromBody] CreateCategoriaDto dto)
    {
        var categoria = await _categoriaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
    }

    /// Actualiza una categoría existente
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoriaDto>> Update(int id, [FromBody] UpdateCategoriaDto dto)
    {
        var categoria = await _categoriaService.UpdateAsync(id, dto);
        
        if (categoria == null)
            return NotFound(new { message = $"Categoría con ID {id} no encontrada" });
        
        return Ok(categoria);
    }

    /// Elimina una categoría
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _categoriaService.DeleteAsync(id);
        
        if (!result)
            return NotFound(new { message = $"Categoría con ID {id} no encontrada" });
        
        return NoContent();
    }
}

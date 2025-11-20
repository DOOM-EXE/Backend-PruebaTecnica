using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VentasApiPrueba.Models.DTOs;
using VentasApiPrueba.Services.Interfaces;

namespace VentasApiPrueba.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    /// Obtiene todos los productos
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAll()
    {
        var productos = await _productoService.GetAllAsync();
        return Ok(productos);
    }

    /// Obtiene un producto por ID
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductoDto>> GetById(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        
        if (producto == null)
            return NotFound(new { message = $"Producto con ID {id} no encontrado" });
        
        return Ok(producto);
    }

    /// Crea un nuevo producto
    [HttpPost]
    [Authorize(Roles = "Admin,Vendedor")]
    public async Task<ActionResult<ProductoDto>> Create([FromBody] CreateProductoDto dto)
    {
        var producto = await _productoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
    }

    /// Actualiza un producto existente
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Vendedor")]
    public async Task<ActionResult<ProductoDto>> Update(int id, [FromBody] UpdateProductoDto dto)
    {
        var producto = await _productoService.UpdateAsync(id, dto);
        
        if (producto == null)
            return NotFound(new { message = $"Producto con ID {id} no encontrado" });
        
        return Ok(producto);
    }

    /// Elimina un producto
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _productoService.DeleteAsync(id);
        
        if (!result)
            return NotFound(new { message = $"Producto con ID {id} no encontrado" });
        
        return NoContent();
    }

    /// Obtiene productos por categoria
    [HttpGet("categoria/{categoriaId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> GetByCategoria(int categoriaId)
    {
        var productos = await _productoService.GetByCategoriaAsync(categoriaId);
        return Ok(productos);
    }

    /// Busca productos por nombre
    [HttpGet("buscar")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> Search([FromQuery] string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return BadRequest(new { message = "El parametro 'nombre' es requerido" });
            
        var productos = await _productoService.SearchByNameAsync(nombre);
        return Ok(productos);
    }
}

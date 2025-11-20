using VentasApiPrueba.Models.DTOs;

namespace VentasApiPrueba.Services.Interfaces;


/// Interfaz para el servicio de productos

public interface IProductoService
{
    Task<List<ProductoDto>> GetAllAsync();
    Task<ProductoDto?> GetByIdAsync(int id);
    Task<ProductoDto> CreateAsync(CreateProductoDto dto);
    Task<ProductoDto?> UpdateAsync(int id, UpdateProductoDto dto);
    Task<bool> DeleteAsync(int id);
    Task<List<ProductoDto>> GetByCategoriaAsync(int categoriaId);
    Task<List<ProductoDto>> SearchByNameAsync(string nombre);
}

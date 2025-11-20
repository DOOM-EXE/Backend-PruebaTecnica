using VentasApiPrueba.Models.DTOs;

namespace VentasApiPrueba.Services.Interfaces;


/// Interfaz para el servicio de categorías

public interface ICategoriaService
{
    Task<List<CategoriaDto>> GetAllAsync();
    Task<CategoriaDto?> GetByIdAsync(int id);
    Task<CategoriaDto> CreateAsync(CreateCategoriaDto dto);
    Task<CategoriaDto?> UpdateAsync(int id, UpdateCategoriaDto dto);
    Task<bool> DeleteAsync(int id);
}

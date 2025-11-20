using VentasApiPrueba.Models.DTOs;

namespace VentasApiPrueba.Services.Interfaces;


/// Interfaz para el servicio de ventas

public interface IVentaService
{
    Task<List<VentaDto>> GetAllAsync();
    Task<VentaDto?> GetByIdAsync(int id);
    Task<VentaDto> CreateAsync(CreateVentaDto dto, int? usuarioId);
    Task<VentaDto?> UpdateAsync(int id, UpdateVentaDto dto);
    Task<bool> CancelAsync(int id);
    Task<List<VentaDto>> GetByClienteAsync(int clienteId);
    Task<List<VentaDto>> GetByFechaRangeAsync(DateTime inicio, DateTime fin);
    Task<List<VentaDto>> GetByEstadoAsync(string estado);
    Task<decimal> GetTotalVentasAsync(DateTime? inicio, DateTime? fin);
}

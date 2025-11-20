using VentasApiPrueba.Models.DTOs;

namespace VentasApiPrueba.Services.Interfaces;


/// Interfaz para el servicio de clientes

public interface IClienteService
{
    Task<List<ClienteDto>> GetAllAsync();
    Task<ClienteDto?> GetByIdAsync(int id);
    Task<ClienteDto> CreateAsync(CreateClienteDto dto);
    Task<ClienteDto?> UpdateAsync(int id, UpdateClienteDto dto);
    Task<bool> DeleteAsync(int id);
    Task<ClienteDto?> GetByEmailAsync(string email);
    Task<List<VentaDto>> GetVentasByClienteAsync(int clienteId);
}

using VentasApiPrueba.Models.DTOs;

namespace VentasApiPrueba.Services.Interfaces;


/// Interfaz para el servicio de autenticación

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<UsuarioDto?> GetUsuarioByUsernameAsync(string username);
    Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword);
    Task<IEnumerable<UsuarioDto>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
    Task<UsuarioDto?> UpdateAsync(int id, UpdateUserDto dto);
    Task<UsuarioDto?> GetByIdAsync(int id);
}

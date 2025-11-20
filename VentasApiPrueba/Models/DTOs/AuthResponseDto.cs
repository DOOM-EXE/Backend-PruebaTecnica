namespace VentasApiPrueba.Models.DTOs;


/// DTO para la respuesta de autenticación (con JWT)

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

namespace VentasApiPrueba.Models.DTOs;


/// DTO para mostrar información de un usuario

public class UsuarioDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public bool Activo { get; set; }
}

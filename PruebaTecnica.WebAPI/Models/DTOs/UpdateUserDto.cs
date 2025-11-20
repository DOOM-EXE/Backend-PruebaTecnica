using System.ComponentModel.DataAnnotations;

namespace VentasApiPrueba.Models.DTOs;

/// DTO para actualizar un usuario por parte de un Admin
public class UpdateUserDto
{
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El username debe tener entre 3 y 50 caracteres")]
    public string? Username { get; set; }

    [EmailAddress(ErrorMessage = "El email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string? Email { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string? Password { get; set; }

    public string? Rol { get; set; }

    public bool? Activo { get; set; }
}

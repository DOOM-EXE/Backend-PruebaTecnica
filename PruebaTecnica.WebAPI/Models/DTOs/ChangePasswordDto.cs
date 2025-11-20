using System.ComponentModel.DataAnnotations;

namespace VentasApiPrueba.Models.DTOs;


/// DTO para cambiar la contraseña de un usuario

public class ChangePasswordDto
{
    [Required(ErrorMessage = "La contraseña actual es requerida")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string NewPassword { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace VentasApiPrueba.Models.DTOs;


/// DTO para crear un nuevo cliente

public class CreateClienteDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es requerido")]
    [Phone(ErrorMessage = "El teléfono no es válido")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string Telefono { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "La dirección no puede exceder 300 caracteres")]
    public string? Direccion { get; set; }
}

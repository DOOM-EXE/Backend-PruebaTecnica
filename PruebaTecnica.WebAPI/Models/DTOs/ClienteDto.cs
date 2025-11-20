namespace VentasApiPrueba.Models.DTOs;


/// DTO para mostrar información de un cliente

public class ClienteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }
}

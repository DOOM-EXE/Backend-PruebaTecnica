namespace VentasApiPrueba.Models.Entities;

/// Entidad que representa un cliente en el sistema

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }

    // Propiedades de navegación
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}

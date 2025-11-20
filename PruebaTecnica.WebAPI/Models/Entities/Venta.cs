namespace VentasApiPrueba.Models.Entities;


/// Entidad que representa una venta en el sistema

public class Venta
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int ClienteId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    // Monto pagado hasta el momento (puede ser parcial)
    public decimal Pagado { get; set; }
    public string Estado { get; set; } = "Pendiente"; // Pendiente, Completada, Cancelada
    public string? MetodoPago { get; set; }
    public string? Observaciones { get; set; }
    public int? UsuarioId { get; set; }

    // Propiedades de navegación
    public Cliente Cliente { get; set; } = null!;
    public Usuario? Usuario { get; set; }
    public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
}

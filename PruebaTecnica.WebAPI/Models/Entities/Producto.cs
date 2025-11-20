namespace VentasApiPrueba.Models.Entities;

/// Entidad que representa un producto en el sistema
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int? CategoriaId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public bool Activo { get; set; }

    // Propiedades de navegación
    public Categoria? Categoria { get; set; }
    public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
}

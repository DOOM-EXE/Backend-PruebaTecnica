namespace VentasApiPrueba.Models.Entities;

/// Entidad que representa una categoría de productos
public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Propiedades de navegación
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}

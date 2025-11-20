namespace VentasApiPrueba.Models.DTOs;


/// DTO para mostrar información de un producto

public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int? CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

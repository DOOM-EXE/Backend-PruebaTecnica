using System.ComponentModel.DataAnnotations;

namespace VentasApiPrueba.Models.DTOs;


/// DTO para crear un detalle de venta

public class CreateDetalleVentaDto
{
    [Required(ErrorMessage = "El ID del producto es requerido")]
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }
}

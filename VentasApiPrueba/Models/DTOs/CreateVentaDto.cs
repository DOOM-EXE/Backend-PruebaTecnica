using System.ComponentModel.DataAnnotations;

namespace VentasApiPrueba.Models.DTOs;


/// DTO para crear una nueva venta

public class CreateVentaDto
{
    [Required(ErrorMessage = "El ID del cliente es requerido")]
    public int ClienteId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El descuento no puede ser negativo")]
    public decimal Descuento { get; set; } = 0;

    [StringLength(50, ErrorMessage = "El método de pago no puede exceder 50 caracteres")]
    public string? MetodoPago { get; set; }

    [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
    public string? Observaciones { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El monto pagado no puede ser negativo")]
    public decimal Pagado { get; set; } = 0;

    [Required(ErrorMessage = "Los detalles de la venta son requeridos")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
    public List<CreateDetalleVentaDto> Detalles { get; set; } = new();
}

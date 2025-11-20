using System.ComponentModel.DataAnnotations;

namespace VentasApiPrueba.Models.DTOs;


/// DTO para actualizar una venta existente

public class UpdateVentaDto
{
    [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
    public string? Estado { get; set; }

    [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
    public string? Observaciones { get; set; }

    // Permitir enviar un abono/pago adicional para la venta
    [Range(0, double.MaxValue, ErrorMessage = "El monto de abono no puede ser negativo")]
    public decimal? Abono { get; set; }
}

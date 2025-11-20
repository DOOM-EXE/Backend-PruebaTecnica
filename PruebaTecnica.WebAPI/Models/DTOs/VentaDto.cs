namespace VentasApiPrueba.Models.DTOs;

/// DTO para mostrar informacion de una venta
public class VentaDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? MetodoPago { get; set; }
    public string? Observaciones { get; set; }
    // Monto ya pagado (puede ser parcial)
    public decimal Pagado { get; set; }

    // Saldo restante (Total - Pagado)
    public decimal Saldo { get; set; }

    // Información del usuario que creó la venta
    public string? UsuarioNombre { get; set; }
    public string? UsuarioRol { get; set; }

    public List<DetalleVentaDto> DetallesVenta { get; set; } = new();
}

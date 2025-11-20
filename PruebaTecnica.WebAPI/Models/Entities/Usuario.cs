namespace VentasApiPrueba.Models.Entities;

/// Entidad que representa un usuario del sistema (para autenticación)

public class Usuario
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Visualizador"; // Admin, Vendedor, Visualizador
    public DateTime FechaCreacion { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public bool Activo { get; set; }

    // Propiedades de navegación
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}

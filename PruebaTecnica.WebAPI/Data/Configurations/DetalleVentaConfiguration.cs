using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentasApiPrueba.Models.Entities;

namespace VentasApiPrueba.Data.Configurations;

/// <summary>
/// Configuración de la entidad DetalleVenta
/// </summary>
public class DetalleVentaConfiguration : IEntityTypeConfiguration<DetalleVenta>
{
    public void Configure(EntityTypeBuilder<DetalleVenta> builder)
    {
        // Tabla
        builder.ToTable("DetalleVenta");

        // Clave primaria
        builder.HasKey(d => d.Id);

        // Propiedades
        builder.Property(d => d.Cantidad)
            .IsRequired();

        builder.Property(d => d.PrecioUnitario)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(d => d.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        // Relaciones
        builder.HasOne(d => d.Venta)
            .WithMany(v => v.DetallesVenta)
            .HasForeignKey(d => d.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Producto)
            .WithMany(p => p.DetallesVenta)
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(d => d.VentaId);
        builder.HasIndex(d => d.ProductoId);
    }
}

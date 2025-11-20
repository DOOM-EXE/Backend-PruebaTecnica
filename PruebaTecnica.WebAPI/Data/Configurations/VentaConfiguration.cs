using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentasApiPrueba.Models.Entities;

namespace VentasApiPrueba.Data.Configurations;


/// Configuración de la entidad Venta

public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        // Configurar tabla
        builder.ToTable("Venta");

        // Clave primaria
        builder.HasKey(v => v.Id);

        // Propiedades
        builder.Property(v => v.Fecha)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(v => v.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(v => v.Descuento)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(v => v.Total)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(v => v.Pagado)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(v => v.Estado)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Pendiente");

        builder.Property(v => v.MetodoPago)
            .HasMaxLength(50);

        builder.Property(v => v.Observaciones)
            .HasMaxLength(500);

        // Relaciones
        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Ventas)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Usuario)
            .WithMany(u => u.Ventas)
            .HasForeignKey(v => v.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(v => v.DetallesVenta)
            .WithOne(dv => dv.Venta)
            .HasForeignKey(dv => dv.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(v => v.ClienteId);
        builder.HasIndex(v => v.Fecha);
        builder.HasIndex(v => v.Estado);
        builder.HasIndex(v => v.UsuarioId);

        // Constraints
        builder.HasCheckConstraint("CK_Venta_Estado", 
            "[Estado] IN ('Pendiente', 'Completada', 'Cancelada')");
    }
}

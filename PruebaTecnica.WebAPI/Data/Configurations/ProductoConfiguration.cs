using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentasApiPrueba.Models.Entities;

namespace VentasApiPrueba.Data.Configurations;


/// Configuración de la entidad Producto

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        // Configurar tabla
        builder.ToTable("Producto");

        // Clave primaria
        builder.HasKey(p => p.Id);

        // Propiedades
        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Descripcion)
            .HasMaxLength(1000);

        builder.Property(p => p.Precio)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Stock)
            .IsRequired();

        builder.Property(p => p.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.FechaActualizacion);

        builder.Property(p => p.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Relaciones
        builder.HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.DetallesVenta)
            .WithOne(dv => dv.Producto)
            .HasForeignKey(dv => dv.ProductoId)
            .OnDelete(DeleteBehavior.NoAction);

        // Índices
        builder.HasIndex(p => p.Nombre);
        builder.HasIndex(p => p.CategoriaId);
        builder.HasIndex(p => p.Activo);
    }
}

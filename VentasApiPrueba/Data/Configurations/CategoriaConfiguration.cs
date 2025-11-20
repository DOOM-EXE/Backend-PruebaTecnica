using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentasApiPrueba.Models.Entities;

namespace VentasApiPrueba.Data.Configurations;


/// Configuración de la entidad Categoria

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        // Configurar tabla
        builder.ToTable("Categoria");

        // Clave primaria
        builder.HasKey(c => c.Id);

        // Propiedades
        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Descripcion)
            .HasMaxLength(500);

        builder.Property(c => c.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // Relaciones
        builder.HasMany(c => c.Productos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices
        builder.HasIndex(c => c.Nombre)
            .IsUnique();
    }
}

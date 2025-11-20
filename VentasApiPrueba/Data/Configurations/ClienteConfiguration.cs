using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentasApiPrueba.Models.Entities;

namespace VentasApiPrueba.Data.Configurations;


/// Configuración de la entidad Cliente

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        // Configurar tabla
        builder.ToTable("Cliente");

        // Clave primaria
        builder.HasKey(c => c.Id);

        // Propiedades
        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Telefono)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Direccion)
            .HasMaxLength(300);

        builder.Property(c => c.FechaRegistro)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(c => c.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Relaciones
        builder.HasMany(c => c.Ventas)
            .WithOne(v => v.Cliente)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(c => c.Email)
            .IsUnique();
        builder.HasIndex(c => c.Activo);
    }
}

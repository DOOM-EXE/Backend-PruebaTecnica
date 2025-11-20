using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VentasApiPrueba.Models.Entities;

namespace VentasApiPrueba.Data.Configurations;


/// Configuración de la entidad Usuario

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        // Configurar tabla
        builder.ToTable("Usuario");

        // Clave primaria
        builder.HasKey(u => u.Id);

        // Propiedades
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(u => u.UltimoAcceso);

        builder.Property(u => u.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Relaciones
        builder.HasMany(u => u.Ventas)
            .WithOne(v => v.Usuario)
            .HasForeignKey(v => v.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices
        builder.HasIndex(u => u.Username)
            .IsUnique();
        builder.HasIndex(u => u.Email)
            .IsUnique();
        builder.HasIndex(u => u.Activo);

        // Constraints
        builder.HasCheckConstraint("CK_Usuario_Rol", 
            "[Rol] IN ('Admin', 'Vendedor', 'Visualizador')");
    }
}

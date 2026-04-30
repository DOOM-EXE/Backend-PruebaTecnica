using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VentasApiPrueba.Models.Entities;

namespace VentasApiPrueba.Data
{
    /// <summary>
    /// Seeder to populate the database with many sample records for development/testing.
    /// </summary>
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // If database already has data we skip seeding
            if (context.Categorias.Any() || context.Productos.Any() || context.Clientes.Any() || context.Usuarios.Any() || context.Ventas.Any())
            {
                return;
            }

            var now = DateTime.UtcNow;
            var rand = new Random(12345);

            // 1) Seed categorias
            var categorias = new List<Categoria>();
            for (int i = 1; i <= 12; i++)
            {
                categorias.Add(new Categoria
                {
                    Nombre = $"Categoria {i}",
                    Descripcion = $"Descripcion de categoria {i}",
                    Activo = true,
                    FechaCreacion = now
                });
            }
            await context.Categorias.AddRangeAsync(categorias);
            await context.SaveChangesAsync();

            // 2) Seed productos (por ejemplo 1000 productos)
            var productos = new List<Producto>();
            int productosCount = 1000;
            for (int i = 1; i <= productosCount; i++)
            {
                var precio = Math.Round((decimal)(rand.NextDouble() * 1000.0 + 1.0), 2);
                var categoria = categorias[rand.Next(categorias.Count)];
                productos.Add(new Producto
                {
                    Nombre = $"Producto {i}",
                    Descripcion = $"Descripcion del producto {i}",
                    Precio = precio,
                    Stock = rand.Next(0, 500),
                    CategoriaId = categoria.Id,
                    FechaCreacion = now,
                    Activo = true
                });
                // Save in chunks to avoid large memory usage
                if (productos.Count % 200 == 0)
                {
                    await context.Productos.AddRangeAsync(productos);
                    await context.SaveChangesAsync();
                    productos.Clear();
                }
            }
            if (productos.Count > 0)
            {
                await context.Productos.AddRangeAsync(productos);
                await context.SaveChangesAsync();
                productos.Clear();
            }

            // Reload all products (now have ids)
            var allProducts = context.Productos.ToList();

            // 3) Seed clientes
            var clientes = new List<Cliente>();
            int clientesCount = 500;
            for (int i = 1; i <= clientesCount; i++)
            {
                clientes.Add(new Cliente
                {
                    Nombre = $"Cliente {i}",
                    Email = $"cliente{i}@example.com",
                    Telefono = $"+100000{i:D6}",
                    Direccion = $"Direccion {i}",
                    FechaRegistro = now.AddDays(-rand.Next(0, 365)),
                    Activo = true
                });
            }
            await context.Clientes.AddRangeAsync(clientes);
            await context.SaveChangesAsync();

            // 4) Seed usuarios
            var usuarios = new List<Usuario>
            {
                new Usuario { Username = "admin", Email = "admin@example.com", PasswordHash = VentasApiPrueba.Helpers.PasswordHasher.HashPassword("Admin123!"), Rol = "Admin", FechaCreacion = now, Activo = true },
                new Usuario { Username = "vendedor1", Email = "vendedor1@example.com", PasswordHash = VentasApiPrueba.Helpers.PasswordHasher.HashPassword("Vendedor123!"), Rol = "Vendedor", FechaCreacion = now, Activo = true },
                new Usuario { Username = "viewer", Email = "viewer@example.com", PasswordHash = VentasApiPrueba.Helpers.PasswordHasher.HashPassword("Viewer123!"), Rol = "Visualizador", FechaCreacion = now, Activo = true }
            };
            await context.Usuarios.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();

            // Reload ids
            var clientesList = context.Clientes.ToList();
            var usuariosList = context.Usuarios.ToList();

            // 5) Seed ventas y detalles
            var ventas = new List<Venta>();
            var detalles = new List<DetalleVenta>();
            int ventasCount = 1000;
            for (int i = 1; i <= ventasCount; i++)
            {
                var cliente = clientesList[rand.Next(clientesList.Count)];
                var usuario = usuariosList[rand.Next(usuariosList.Count)];
                var venta = new Venta
                {
                    Fecha = now.AddDays(-rand.Next(0, 365)),
                    ClienteId = cliente.Id,
                    UsuarioId = usuario.Id,
                    Subtotal = 0m,
                    Descuento = 0m,
                    Total = 0m,
                    Pagado = 0m,
                    Estado = "Completada",
                    MetodoPago = (rand.Next(0, 2) == 0) ? "Efectivo" : "Tarjeta",
                    Observaciones = null
                };
                ventas.Add(venta);

                // Create between 1 and 6 detalles
                int detallesPorVenta = rand.Next(1, 6);
                for (int d = 0; d < detallesPorVenta; d++)
                {
                    var producto = allProducts[rand.Next(allProducts.Count)];
                    var cantidad = rand.Next(1, 6);
                    var precioUnit = producto.Precio;
                    var subtotal = Math.Round(precioUnit * cantidad, 2);

                    detalles.Add(new DetalleVenta
                    {
                        Venta = venta,
                        ProductoId = producto.Id,
                        Cantidad = cantidad,
                        PrecioUnitario = precioUnit,
                        Subtotal = subtotal
                    });

                    // actualizar stock en memoria (si existe)
                    producto.Stock = Math.Max(0, producto.Stock - cantidad);
                }

                // Save in chunks
                if (ventas.Count % 100 == 0)
                {
                    await context.Ventas.AddRangeAsync(ventas);
                    await context.SaveChangesAsync();
                    ventas.Clear();
                }
            }

            if (ventas.Count > 0)
            {
                await context.Ventas.AddRangeAsync(ventas);
                await context.SaveChangesAsync();
                ventas.Clear();
            }

            // Add detalles in batches
            var detallesList = detalles;
            const int batchSize = 500;
            for (int i = 0; i < detallesList.Count; i += batchSize)
            {
                var batch = detallesList.Skip(i).Take(batchSize).ToList();
                await context.DetallesVenta.AddRangeAsync(batch);
                await context.SaveChangesAsync();
            }

            // Persist product stock updates
            var modifiedProducts = context.Productos.Where(p => p.Stock >= 0).ToList();
            context.Productos.UpdateRange(modifiedProducts);
            await context.SaveChangesAsync();
        }
    }
}

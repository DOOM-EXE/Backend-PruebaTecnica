using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VentasApiPrueba.Data;
using VentasApiPrueba.Middleware;
using VentasApiPrueba.Models.DTOs;
using VentasApiPrueba.Models.Entities;
using VentasApiPrueba.Services.Interfaces;

namespace VentasApiPrueba.Services.Implementations;


/// Servicio para gestionar ventas

public class VentaService : IVentaService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public VentaService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<VentaDto>> GetAllAsync()
    {
        var ventas = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.DetallesVenta)
                .ThenInclude(dv => dv.Producto)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();

        return _mapper.Map<List<VentaDto>>(ventas);
    }

    public async Task<VentaDto?> GetByIdAsync(int id)
    {
        var venta = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.DetallesVenta)
                .ThenInclude(dv => dv.Producto)
            .FirstOrDefaultAsync(v => v.Id == id);

        return venta == null ? null : _mapper.Map<VentaDto>(venta);
    }

    public async Task<VentaDto> CreateAsync(CreateVentaDto dto, int? usuarioId)
    {
        // Validar que el cliente existe
        var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
        if (cliente == null)
            throw new NotFoundException($"Cliente con ID {dto.ClienteId} no encontrado");

        if (!cliente.Activo)
            throw new BadRequestException("El cliente está inactivo");

        // Validar stock de cada producto
        foreach (var detalleDto in dto.Detalles)
        {
            var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
            
            if (producto == null)
                throw new NotFoundException($"Producto con ID {detalleDto.ProductoId} no encontrado");

            if (!producto.Activo)
                throw new BadRequestException($"El producto {producto.Nombre} está inactivo");

            if (producto.Stock < detalleDto.Cantidad)
                throw new BadRequestException($"Stock insuficiente para {producto.Nombre}. Disponible: {producto.Stock}");
        }

    // Crear venta
    var venta = _mapper.Map<Venta>(dto);
    // Asignar el usuario que crea la venta (si se proporciona)
    venta.UsuarioId = usuarioId;
        
        // Crear detalles y calcular totales
        decimal subtotal = 0;
        foreach (var detalleDto in dto.Detalles)
        {
            var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
            
            var detalle = new DetalleVenta
            {
                ProductoId = detalleDto.ProductoId,
                Cantidad = detalleDto.Cantidad,
                PrecioUnitario = producto!.Precio,
                Subtotal = detalleDto.Cantidad * producto.Precio
            };

            subtotal += detalle.Subtotal;
            venta.DetallesVenta.Add(detalle);

            // Actualizar stock
            producto.Stock -= detalleDto.Cantidad;
        }

        venta.Subtotal = subtotal;
        // Aplicar descuento
        venta.Total = decimal.Round(subtotal - dto.Descuento, 2, MidpointRounding.AwayFromZero);

        // Registrar pago inicial (si se envió)
        venta.Pagado = decimal.Round(dto.Pagado, 2, MidpointRounding.AwayFromZero);

        // Determinar estado según pago
        if (venta.Pagado >= venta.Total)
            venta.Estado = "Completada";
        else
            venta.Estado = "Pendiente";

        _context.Ventas.Add(venta);
        await _context.SaveChangesAsync();

        // Recargar la venta con todas las relaciones
        var ventaCreada = await GetByIdAsync(venta.Id);
        return ventaCreada!;
    }

    public async Task<VentaDto?> UpdateAsync(int id, UpdateVentaDto dto)
    {
        var venta = await _context.Ventas.FindAsync(id);
        
        if (venta == null)
            return null;

        // Si se envía un abono, aplicar al Pagado
        if (dto.Abono.HasValue && dto.Abono.Value > 0)
        {
            venta.Pagado += decimal.Round(dto.Abono.Value, 2, MidpointRounding.AwayFromZero);
            // No permitir que Pagado supere Total
            if (venta.Pagado > venta.Total)
                venta.Pagado = venta.Total;
        }

        if (!string.IsNullOrEmpty(dto.Estado))
            venta.Estado = dto.Estado;

        if (!string.IsNullOrEmpty(dto.Observaciones))
            venta.Observaciones = dto.Observaciones;

        // Recalcular estado si no fue explicitado
        if (string.IsNullOrEmpty(dto.Estado))
        {
            venta.Estado = venta.Pagado >= venta.Total ? "Completada" : "Pendiente";
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> CancelAsync(int id)
    {
        var venta = await _context.Ventas
            .Include(v => v.DetallesVenta)
                .ThenInclude(dv => dv.Producto)
            .FirstOrDefaultAsync(v => v.Id == id);
        
        if (venta == null)
            return false;

        // Si ya está cancelada, no hacemos nada (idempotente)
        if (venta.Estado == "Cancelada")
            return true;

        // Usar transacción para asegurar atomicidad al devolver stock y cambiar estado
        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            // Devolver stock
            foreach (var detalle in venta.DetallesVenta)
            {
                // Asegurarnos de que el producto esté cargado
                if (detalle.Producto != null)
                {
                    detalle.Producto.Stock += detalle.Cantidad;
                }
            }

            venta.Estado = "Cancelada";
            await _context.SaveChangesAsync();

            await tx.CommitAsync();
            return true;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<List<VentaDto>> GetByClienteAsync(int clienteId)
    {
        var ventas = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.DetallesVenta)
                .ThenInclude(dv => dv.Producto)
            .Where(v => v.ClienteId == clienteId)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();

        return _mapper.Map<List<VentaDto>>(ventas);
    }

    public async Task<List<VentaDto>> GetByFechaRangeAsync(DateTime inicio, DateTime fin)
    {
        var ventas = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.DetallesVenta)
                .ThenInclude(dv => dv.Producto)
            .Where(v => v.Fecha >= inicio && v.Fecha <= fin)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();

        return _mapper.Map<List<VentaDto>>(ventas);
    }

    public async Task<List<VentaDto>> GetByEstadoAsync(string estado)
    {
        var ventas = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.DetallesVenta)
                .ThenInclude(dv => dv.Producto)
            .Where(v => v.Estado == estado)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();

        return _mapper.Map<List<VentaDto>>(ventas);
    }

    public async Task<decimal> GetTotalVentasAsync(DateTime? inicio, DateTime? fin)
    {
        var query = _context.Ventas
            .Where(v => v.Estado == "Completada");

        if (inicio.HasValue)
            query = query.Where(v => v.Fecha >= inicio.Value);

        if (fin.HasValue)
            query = query.Where(v => v.Fecha <= fin.Value);

        return await query.SumAsync(v => v.Total);
    }
}

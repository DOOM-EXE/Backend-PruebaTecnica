using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VentasApiPrueba.Data;
using VentasApiPrueba.Middleware;
using VentasApiPrueba.Models.DTOs;
using VentasApiPrueba.Models.Entities;
using VentasApiPrueba.Services.Interfaces;

namespace VentasApiPrueba.Services.Implementations;


/// Servicio para gestionar productos

public class ProductoService : IProductoService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ProductoService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProductoDto>> GetAllAsync()
    {
        var productos = await _context.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Activo)
            .ToListAsync();

        return _mapper.Map<List<ProductoDto>>(productos);
    }

    public async Task<ProductoDto?> GetByIdAsync(int id)
    {
        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

        return producto == null ? null : _mapper.Map<ProductoDto>(producto);
    }

    public async Task<ProductoDto> CreateAsync(CreateProductoDto dto)
    {
        // Validar categoría si se especifica
        if (dto.CategoriaId.HasValue)
        {
            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.Id == dto.CategoriaId.Value && c.Activo);
            
            if (!categoriaExiste)
                throw new NotFoundException($"Categoría con ID {dto.CategoriaId.Value} no encontrada");
        }

        var producto = _mapper.Map<Producto>(dto);
        
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductoDto>(producto);
    }

    public async Task<ProductoDto?> UpdateAsync(int id, UpdateProductoDto dto)
    {
        var producto = await _context.Productos.FindAsync(id);
        
        if (producto == null)
            return null;

        // Validar categoría si se especifica
        if (dto.CategoriaId.HasValue)
        {
            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.Id == dto.CategoriaId.Value && c.Activo);
            
            if (!categoriaExiste)
                throw new NotFoundException($"Categoría con ID {dto.CategoriaId.Value} no encontrada");
        }

        _mapper.Map(dto, producto);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductoDto>(producto);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        
        if (producto == null)
            return false;

        // Eliminación lógica
        producto.Activo = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ProductoDto>> GetByCategoriaAsync(int categoriaId)
    {
        var productos = await _context.Productos
            .Include(p => p.Categoria)
            .Where(p => p.CategoriaId == categoriaId && p.Activo)
            .ToListAsync();

        return _mapper.Map<List<ProductoDto>>(productos);
    }

    public async Task<List<ProductoDto>> SearchByNameAsync(string nombre)
    {
        var productos = await _context.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Nombre.Contains(nombre) && p.Activo)
            .ToListAsync();

        return _mapper.Map<List<ProductoDto>>(productos);
    }
}

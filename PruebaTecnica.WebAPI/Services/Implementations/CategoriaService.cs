using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VentasApiPrueba.Data;
using VentasApiPrueba.Middleware;
using VentasApiPrueba.Models.DTOs;
using VentasApiPrueba.Models.Entities;
using VentasApiPrueba.Services.Interfaces;

namespace VentasApiPrueba.Services.Implementations;


/// Servicio para gestionar categorías

public class CategoriaService : ICategoriaService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CategoriaService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CategoriaDto>> GetAllAsync()
    {
        var categorias = await _context.Categorias
            .Where(c => c.Activo)
            .ToListAsync();

        return _mapper.Map<List<CategoriaDto>>(categorias);
    }

    public async Task<CategoriaDto?> GetByIdAsync(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        return categoria == null ? null : _mapper.Map<CategoriaDto>(categoria);
    }

    public async Task<CategoriaDto> CreateAsync(CreateCategoriaDto dto)
    {
        // Verificar nombre duplicado
        var nombreExiste = await _context.Categorias
            .AnyAsync(c => c.Nombre == dto.Nombre);

        if (nombreExiste)
            throw new DuplicateException($"Ya existe una categoría con el nombre {dto.Nombre}");

        var categoria = _mapper.Map<Categoria>(dto);
        
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return _mapper.Map<CategoriaDto>(categoria);
    }

    public async Task<CategoriaDto?> UpdateAsync(int id, UpdateCategoriaDto dto)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        
        if (categoria == null)
            return null;

        // Verificar nombre duplicado (excepto el actual)
        var nombreExiste = await _context.Categorias
            .AnyAsync(c => c.Nombre == dto.Nombre && c.Id != id);

        if (nombreExiste)
            throw new DuplicateException($"Ya existe otra categoría con el nombre {dto.Nombre}");

        _mapper.Map(dto, categoria);
        await _context.SaveChangesAsync();

        return _mapper.Map<CategoriaDto>(categoria);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        
        if (categoria == null)
            return false;

        // Eliminación lógica
        categoria.Activo = false;
        await _context.SaveChangesAsync();

        return true;
    }
}

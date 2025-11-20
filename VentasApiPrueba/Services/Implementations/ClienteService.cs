using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VentasApiPrueba.Data;
using VentasApiPrueba.Middleware;
using VentasApiPrueba.Models.DTOs;
using VentasApiPrueba.Models.Entities;
using VentasApiPrueba.Services.Interfaces;

namespace VentasApiPrueba.Services.Implementations;


/// Servicio para gestionar clientes

public class ClienteService : IClienteService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ClienteService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ClienteDto>> GetAllAsync()
    {
        var clientes = await _context.Clientes
            .Where(c => c.Activo)
            .ToListAsync();

        return _mapper.Map<List<ClienteDto>>(clientes);
    }

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        return cliente == null ? null : _mapper.Map<ClienteDto>(cliente);
    }

    public async Task<ClienteDto> CreateAsync(CreateClienteDto dto)
    {
        // Verificar email duplicado
        var emailExiste = await _context.Clientes
            .AnyAsync(c => c.Email == dto.Email);

        if (emailExiste)
            throw new DuplicateException($"Ya existe un cliente con el email {dto.Email}");

        var cliente = _mapper.Map<Cliente>(dto);
        
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return _mapper.Map<ClienteDto>(cliente);
    }

    public async Task<ClienteDto?> UpdateAsync(int id, UpdateClienteDto dto)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        
        if (cliente == null)
            return null;

        // Verificar email duplicado (excepto el actual)
        var emailExiste = await _context.Clientes
            .AnyAsync(c => c.Email == dto.Email && c.Id != id);

        if (emailExiste)
            throw new DuplicateException($"Ya existe otro cliente con el email {dto.Email}");

        _mapper.Map(dto, cliente);
        await _context.SaveChangesAsync();

        return _mapper.Map<ClienteDto>(cliente);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        
        if (cliente == null)
            return false;

        // Eliminación lógica
        cliente.Activo = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ClienteDto?> GetByEmailAsync(string email)
    {
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Email == email);

        return cliente == null ? null : _mapper.Map<ClienteDto>(cliente);
    }

    public async Task<List<VentaDto>> GetVentasByClienteAsync(int clienteId)
    {
        var ventas = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.DetallesVenta)
                .ThenInclude(dv => dv.Producto)
            .Where(v => v.ClienteId == clienteId)
            .ToListAsync();

        return _mapper.Map<List<VentaDto>>(ventas);
    }
}

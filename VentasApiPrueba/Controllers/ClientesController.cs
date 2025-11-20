using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VentasApiPrueba.Models.DTOs;
using VentasApiPrueba.Services.Interfaces;

namespace VentasApiPrueba.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// Obtiene todos los clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
    {
        var clientes = await _clienteService.GetAllAsync();
        return Ok(clientes);
    }

    /// Obtiene un cliente por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDto>> GetById(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);
        
        if (cliente == null)
            return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
        
        return Ok(cliente);
    }

    /// Crea un nuevo cliente
    [HttpPost]
    [Authorize(Roles = "Admin,Vendedor")]
    public async Task<ActionResult<ClienteDto>> Create([FromBody] CreateClienteDto dto)
    {
        var cliente = await _clienteService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    
    /// Actualiza un cliente existente
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Vendedor")]
    public async Task<ActionResult<ClienteDto>> Update(int id, [FromBody] UpdateClienteDto dto)
    {
        var cliente = await _clienteService.UpdateAsync(id, dto);
        
        if (cliente == null)
            return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
        
        return Ok(cliente);
    }

    /// Elimina un cliente
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _clienteService.DeleteAsync(id);
        
        if (!result)
            return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
        
        return NoContent();
    }

    /// Obtiene las ventas de un cliente
    [HttpGet("{id}/ventas")]
    public async Task<ActionResult<IEnumerable<VentaDto>>> GetVentas(int id)
    {
        var ventas = await _clienteService.GetVentasByClienteAsync(id);
        return Ok(ventas);
    }

    
    /// Busca un cliente por email
    
    [HttpGet("buscar")]
    public async Task<ActionResult<ClienteDto>> SearchByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { message = "El parámetro 'email' es requerido" });
            
        var cliente = await _clienteService.GetByEmailAsync(email);
        
        if (cliente == null)
            return NotFound(new { message = $"Cliente con email {email} no encontrado" });
        
        return Ok(cliente);
    }
}

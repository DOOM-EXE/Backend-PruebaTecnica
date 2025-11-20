using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using VentasApiPrueba.Models.DTOs;
using VentasApiPrueba.Services.Interfaces;

namespace VentasApiPrueba.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VentasController : ControllerBase
{
    private readonly IVentaService _ventaService;

    public VentasController(IVentaService ventaService)
    {
        _ventaService = ventaService;
    }

    /// Obtiene todas las ventas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VentaDto>>> GetAll()
    {
        var ventas = await _ventaService.GetAllAsync();
        return Ok(ventas);
    }

    /// Obtiene una venta por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<VentaDto>> GetById(int id)
    {
        var venta = await _ventaService.GetByIdAsync(id);
        
        if (venta == null)
            return NotFound(new { message = $"Venta con ID {id} no encontrada" });
        
        return Ok(venta);
    }

    
    /// Crea una nueva venta
    
    [HttpPost]
    [Authorize(Roles = "Admin,Vendedor")]
    public async Task<ActionResult<VentaDto>> Create([FromBody] CreateVentaDto dto)
    {
        // Intentar obtener el ID del usuario autenticado desde los claims
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id")?.Value;
        int? usuarioId = null;
        if (int.TryParse(idClaim, out var parsed)) usuarioId = parsed;

        var venta = await _ventaService.CreateAsync(dto, usuarioId);
        return CreatedAtAction(nameof(GetById), new { id = venta.Id }, venta);
    }

    
    /// Actualiza una venta existente
    
    [HttpPut("{id}")]
    public async Task<ActionResult<VentaDto>> Update(int id, [FromBody] UpdateVentaDto dto)
    {
        // Validar modelo
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validación de roles: permitimos Admin o Vendedor para esta acción.
        // Usamos un chequeo robusto de claims porque algunos tokens pueden usar distintos claim types para el rol.
        bool HasRole(string role)
        {
            if (User.IsInRole(role)) return true;
            var possibleRoleClaims = new[] { ClaimTypes.Role, "role", "roles", "rol" };
            return User.Claims.Any(c => possibleRoleClaims.Contains(c.Type) && string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase));
        }

        // Requerir que el usuario sea Admin o Vendedor
        if (!HasRole("Admin") && !HasRole("Vendedor"))
            return Forbid();

        // Si el cambio de estado es a "Cancelada" solo Admin puede hacerlo
        if (!string.IsNullOrEmpty(dto.Estado) && dto.Estado == "Cancelada" && !HasRole("Admin"))
        {
            return Forbid(); // 403
        }

        var venta = await _ventaService.UpdateAsync(id, dto);
        
        if (venta == null)
            return NotFound(new { message = $"Venta con ID {id} no encontrada" });
        
        return Ok(venta);
    }

    
    /// Cancela una venta
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Cancel(int id)
    {
        var result = await _ventaService.CancelAsync(id);
        
        if (!result)
            return NotFound(new { message = $"Venta con ID {id} no encontrada" });
        
        return NoContent();
    }

    
    /// Obtiene ventas por cliente
    
    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<VentaDto>>> GetByCliente(int clienteId)
    {
        var ventas = await _ventaService.GetByClienteAsync(clienteId);
        return Ok(ventas);
    }

    
    /// Obtiene ventas por rango de fechas
    
    [HttpGet("fecha")]
    public async Task<ActionResult<IEnumerable<VentaDto>>> GetByFecha(
        [FromQuery] DateTime inicio, 
        [FromQuery] DateTime fin)
    {
        if (inicio > fin)
            return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha fin" });
            
        var ventas = await _ventaService.GetByFechaRangeAsync(inicio, fin);
        return Ok(ventas);
    }

    
    /// Obtiene ventas por estado
    
    [HttpGet("estado/{estado}")]
    public async Task<ActionResult<IEnumerable<VentaDto>>> GetByEstado(string estado)
    {
        var estadosValidos = new[] { "Pendiente", "Completada", "Cancelada" };
        if (!estadosValidos.Contains(estado))
            return BadRequest(new { message = "Estado no válido. Valores permitidos: Pendiente, Completada, Cancelada" });
            
        var ventas = await _ventaService.GetByEstadoAsync(estado);
        return Ok(ventas);
    }

    
    /// Obtiene el total de ventas en un rango de fechas
    
    [HttpGet("reporte/total")]
    public async Task<ActionResult> GetTotalVentas(
        [FromQuery] DateTime? inicio, 
        [FromQuery] DateTime? fin)
    {
        var total = await _ventaService.GetTotalVentasAsync(inicio, fin);
        return Ok(new 
        { 
            total, 
            inicio = inicio ?? DateTime.MinValue, 
            fin = fin ?? DateTime.Now 
        });
    }
}

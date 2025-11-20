using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using VentasApiPrueba.Models.DTOs;
using VentasApiPrueba.Services.Interfaces;

namespace VentasApiPrueba.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// Registra un nuevo usuario
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        // Validar rol permitido
        var rolesPermitidos = new[] { "Admin", "Vendedor", "Visualizador" };
        if (!rolesPermitidos.Contains(dto.Rol))
            return BadRequest(new { message = "Rol no permitido. Valores válidos: Admin, Vendedor, Visualizador" });

        // Si se intenta crear un usuario con rol Admin o Vendedor, solo un Admin puede hacerlo.
        if (dto.Rol == "Admin" || dto.Rol == "Vendedor")
        {
            // Si no está autenticado -> no puede crear estos roles
            if (!User.Identity?.IsAuthenticated ?? true)
                return Unauthorized(new { message = "Solo administradores pueden crear usuarios con rol Admin o Vendedor" });

            // Chequeo robusto de claim de rol (por si el token usa distintos claim types)
            bool HasRole(string role)
            {
                if (User.IsInRole(role)) return true;
                var possibleRoleClaims = new[] { System.Security.Claims.ClaimTypes.Role, "role", "roles", "rol" };
                return User.Claims.Any(c => possibleRoleClaims.Contains(c.Type) && string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase));
            }

            if (!HasRole("Admin"))
                return Forbid();
        }

        var response = await _authService.RegisterAsync(dto);

        if (response == null)
            return BadRequest(new { message = "El usuario o email ya existe" });

        return Ok(response);
    }

    /// Inicia sesion con usuario y contraseña
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        
        if (response == null)
            return Unauthorized(new { message = "Credenciales inválidas" });
        
        return Ok(response);
    }

    /// Obtiene la informacion del usuario autenticado
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> GetCurrentUser()
    {
        var username = User.Identity?.Name;
        
        if (string.IsNullOrEmpty(username))
            return Unauthorized(new { message = "No se pudo identificar al usuario" });
        
        var usuario = await _authService.GetUsuarioByUsernameAsync(username);
        
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado" });
        
        return Ok(usuario);
    }

    /// Obtiene la lista de usuarios (solo Admin)
    [HttpGet("users")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsers()
    {
        // Chequeo robusto de rol Admin (evita depender sólo del attribute Roles)
        bool HasRole(string role)
        {
            if (User.IsInRole(role)) return true;
            var possibleRoleClaims = new[] { System.Security.Claims.ClaimTypes.Role, "role", "roles", "rol" };
            return User.Claims.Any(c => possibleRoleClaims.Contains(c.Type) && string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase));
        }

        if (!HasRole("Admin"))
            return Forbid();

        var usuarios = await _authService.GetAllAsync();
        return Ok(usuarios);
    }

    /// Obtiene un usuario por id (solo Admin)
    [HttpGet("users/{id}")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> GetUserById(int id)
    {
        // Chequeo robusto de rol Admin
        bool HasRole(string role)
        {
            if (User.IsInRole(role)) return true;
            var possibleRoleClaims = new[] { System.Security.Claims.ClaimTypes.Role, "role", "roles", "rol" };
            return User.Claims.Any(c => possibleRoleClaims.Contains(c.Type) && string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase));
        }

        if (!HasRole("Admin"))
            return Forbid();

        var usuario = await _authService.GetByIdAsync(id);
        if (usuario == null)
            return NotFound(new { message = $"Usuario con ID {id} no encontrado" });

        return Ok(usuario);
    }

    /// Elimina un usuario por id (solo Admin)
    [HttpDelete("users/{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteUser(int id)
    {
        // Chequeo robusto de rol Admin
        bool HasRole(string role)
        {
            if (User.IsInRole(role)) return true;
            var possibleRoleClaims = new[] { System.Security.Claims.ClaimTypes.Role, "role", "roles", "rol" };
            return User.Claims.Any(c => possibleRoleClaims.Contains(c.Type) && string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase));
        }

        if (!HasRole("Admin"))
            return Forbid();

        // Evitar que un admin se elimine a sí mismo
        var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst("id")?.Value;
        if (int.TryParse(idClaim, out var currentUserId) && currentUserId == id)
            return BadRequest(new { message = "No puedes eliminar tu propio usuario" });

        var deleted = await _authService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Usuario con ID {id} no encontrado" });

        return NoContent();
    }

    /// Actualiza un usuario (solo Admin)
    [HttpPut("users/{id}")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        // Chequeo robusto de rol Admin
        bool HasRole(string role)
        {
            if (User.IsInRole(role)) return true;
            var possibleRoleClaims = new[] { System.Security.Claims.ClaimTypes.Role, "role", "roles", "rol" };
            return User.Claims.Any(c => possibleRoleClaims.Contains(c.Type) && string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase));
        }

        if (!HasRole("Admin"))
            return Forbid();

        // Validar rol solicitado si viene
        if (!string.IsNullOrEmpty(dto.Rol))
        {
            var rolesPermitidos = new[] { "Admin", "Vendedor", "Visualizador" };
            if (!rolesPermitidos.Contains(dto.Rol))
                return BadRequest(new { message = "Rol no permitido. Valores válidos: Admin, Vendedor, Visualizador" });
        }

        var updated = await _authService.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(new { message = $"Usuario con ID {id} no encontrado" });

        return Ok(updated);
    }

    /// Cambia la contraseña del usuario autenticado
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var username = User.Identity?.Name;
        
        if (string.IsNullOrEmpty(username))
            return Unauthorized(new { message = "No se pudo identificar al usuario" });
        
        var result = await _authService.ChangePasswordAsync(username, dto.CurrentPassword, dto.NewPassword);
        
        if (!result)
            return BadRequest(new { message = "Contraseña actual incorrecta" });
        
        return Ok(new { message = "Contraseña actualizada exitosamente" });
    }

    /// Valida si el token es válido (para verificación de frontend)
    [HttpGet("validate")]
    [Authorize]
    public ActionResult ValidateToken()
    {
        return Ok(new 
        { 
            valid = true, 
            username = User.Identity?.Name,
            roles = User.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList()
        });
    }
}

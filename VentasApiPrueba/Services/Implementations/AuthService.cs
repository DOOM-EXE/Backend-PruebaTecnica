using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VentasApiPrueba.Data;
using VentasApiPrueba.Middleware;
using VentasApiPrueba.Helpers;
using VentasApiPrueba.Models.DTOs;
using VentasApiPrueba.Models.Entities;
using VentasApiPrueba.Services.Interfaces;

namespace VentasApiPrueba.Services.Implementations;


/// Servicio para autenticacion y gestión de usuarios

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        // Verificar si el username ya existe
        var usernameExiste = await _context.Usuarios
            .AnyAsync(u => u.Username == dto.Username);

        if (usernameExiste)
            return null;

        // Verificar si el email ya existe
        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email == dto.Email);

        if (emailExiste)
            return null;

        // Crear usuario
        var usuario = _mapper.Map<Usuario>(dto);
        usuario.PasswordHash = PasswordHasher.HashPassword(dto.Password);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Generar token
        var token = GenerateJwtToken(usuario);
        var expiresAt = DateTime.UtcNow.AddMinutes(
            _configuration.GetValue<int>("Jwt:ExpiresInMinutes"));

        return new AuthResponseDto
        {
            Token = token,
            Username = usuario.Username,
            Email = usuario.Email,
            Rol = usuario.Rol,
            ExpiresAt = expiresAt
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        // Buscar usuario por username
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (usuario == null)
            return null;

        // Verificar contraseña
        if (!PasswordHasher.VerifyPassword(dto.Password, usuario.PasswordHash))
            return null;

        // Verificar que esté activo
        if (!usuario.Activo)
            return null;

        // Actualizar último acceso
        usuario.UltimoAcceso = DateTime.Now;
        await _context.SaveChangesAsync();

        // Generar token
        var token = GenerateJwtToken(usuario);
        var expiresAt = DateTime.UtcNow.AddMinutes(
            _configuration.GetValue<int>("Jwt:ExpiresInMinutes"));

        return new AuthResponseDto
        {
            Token = token,
            Username = usuario.Username,
            Email = usuario.Email,
            Rol = usuario.Rol,
            ExpiresAt = expiresAt
        };
    }

    public async Task<UsuarioDto?> GetUsuarioByUsernameAsync(string username)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Username == username);

        return usuario == null ? null : _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
    {
        var usuarios = await _context.Usuarios.ToListAsync();
        return _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return false;

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UsuarioDto?> UpdateAsync(int id, UpdateUserDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return null;

        // Validaciones: username/email únicos (si se están cambiando)
        if (!string.IsNullOrEmpty(dto.Username) && dto.Username != usuario.Username)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Username == dto.Username && u.Id != id);
            if (existe) throw new DuplicateException("El username ya está en uso");
            usuario.Username = dto.Username;
        }

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != usuario.Email)
        {
            var existeEmail = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.Id != id);
            if (existeEmail) throw new DuplicateException("El email ya está en uso");
            usuario.Email = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            usuario.PasswordHash = PasswordHasher.HashPassword(dto.Password);
        }

        if (!string.IsNullOrEmpty(dto.Rol) && !string.Equals(dto.Rol, usuario.Rol, StringComparison.OrdinalIgnoreCase))
        {
            // Si se está demotando un admin, verificar que exista al menos otro admin
            if (string.Equals(usuario.Rol, "Admin", StringComparison.OrdinalIgnoreCase) && !string.Equals(dto.Rol, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                var admins = await _context.Usuarios.CountAsync(u => u.Rol == "Admin");
                if (admins <= 1)
                    throw new BadRequestException("No se puede demotar al último administrador");
            }

            usuario.Rol = dto.Rol;
        }

        if (dto.Activo.HasValue)
            usuario.Activo = dto.Activo.Value;

        await _context.SaveChangesAsync();
        return _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<UsuarioDto?> GetByIdAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        return usuario == null ? null : _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Username == username);

        if (usuario == null)
            return false;

        // Verificar contraseña actual
        if (!PasswordHasher.VerifyPassword(currentPassword, usuario.PasswordHash))
            return false;

        // Cambiar contraseña
        usuario.PasswordHash = PasswordHasher.HashPassword(newPassword);
        await _context.SaveChangesAsync();

        return true;
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var jwtKey = _configuration["Jwt:Key"]!;
        var jwtIssuer = _configuration["Jwt:Issuer"]!;
        var jwtAudience = _configuration["Jwt:Audience"]!;
        var expiresInMinutes = _configuration.GetValue<int>("Jwt:ExpiresInMinutes");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario.Username),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol),
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

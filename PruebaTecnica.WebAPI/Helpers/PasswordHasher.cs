namespace VentasApiPrueba.Helpers;


/// Helper para hashear y verificar contraseñas usando BCrypt

public static class PasswordHasher
{
    
    /// Hashea una contraseña usando BCrypt
    
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    
    /// Verifica si una contraseña coincide con el hash almacenado
    
    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}

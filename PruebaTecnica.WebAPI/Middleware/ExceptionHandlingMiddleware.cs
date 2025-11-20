using System.Net;
using System.Text.Json;

namespace VentasApiPrueba.Middleware;


/// Middleware para manejo centralizado de excepciones

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió una excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = GetMessage(exception),
            Details = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            response.StackTrace = exception.StackTrace;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        return context.Response.WriteAsync(json);
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            NotFoundException => (int)HttpStatusCode.NotFound,
            BadRequestException => (int)HttpStatusCode.BadRequest,
            UnauthorizedException => (int)HttpStatusCode.Unauthorized,
            ForbiddenException => (int)HttpStatusCode.Forbidden,
            ValidationException => (int)HttpStatusCode.BadRequest,
            DuplicateException => (int)HttpStatusCode.Conflict,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    private static string GetMessage(Exception exception)
    {
        return exception switch
        {
            NotFoundException => "Recurso no encontrado",
            BadRequestException => "Solicitud incorrecta",
            UnauthorizedException => "No autorizado",
            ForbiddenException => "Acceso denegado",
            ValidationException => "Error de validación",
            DuplicateException => "El recurso ya existe",
            InvalidOperationException => "Operacion no valida",
            ArgumentNullException => "Parametro requerido faltante",
            ArgumentException => "Parametro invalido",
            _ => "Error interno del servidor"
        };
    }
}


/// Modelo de respuesta de error

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? StackTrace { get; set; }
}

// Excepciones personalizadas
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

public class DuplicateException : Exception
{
    public DuplicateException(string message) : base(message) { }
}

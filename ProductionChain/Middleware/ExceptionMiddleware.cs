using ProductionChain.Contracts.Exceptions;
using System.Text.Json;

namespace ProductionChain.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, exception.Message),
            ForbiddenException => (StatusCodes.Status403Forbidden, exception.Message),
            InvalidStateException => (StatusCodes.Status409Conflict, exception.Message),
            InsufficientComponentsException => (StatusCodes.Status400BadRequest, exception.Message),
            UpdateStateException => (StatusCodes.Status422UnprocessableEntity, exception.Message),
            AccountNotCreatedException => (StatusCodes.Status409Conflict, exception.Message),
            NotificationException => (StatusCodes.Status404NotFound, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Ошибка сервера.")
        };

        var isDevelopment = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();

        var response = new
        {
            error = message,
            stackTrace = isDevelopment ? exception.StackTrace : null
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

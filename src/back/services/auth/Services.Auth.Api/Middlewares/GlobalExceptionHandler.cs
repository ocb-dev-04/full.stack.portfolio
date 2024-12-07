using Microsoft.AspNetCore.Mvc;

namespace Services.Auth.Api.Middlewares;

public sealed class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        RequestDelegate next,
        ILogger<GlobalExceptionHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(next, nameof(next));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

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
            _logger.LogError(ex, "Exception ocurred: {Message}", ex.Message);

            ProblemDetails problemDetails = new ()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}

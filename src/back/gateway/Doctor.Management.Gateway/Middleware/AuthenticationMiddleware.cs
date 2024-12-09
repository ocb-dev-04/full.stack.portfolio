using Refit;
using System.Text.Json;
using Doctor.Management.Gateway.AuthClient;

namespace Doctor.Management.Gateway.Middleware;

public sealed class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAuthClient _authClient;

    private readonly static string _authServicesIdentifier = "auth";
    private readonly static string _authHeaderIdentifier = "Authorization";

    public AuthenticationMiddleware(RequestDelegate next, IAuthClient authClient)
    {
        _next = next;
        _authClient = authClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        HttpRequest request = context.Request;

        bool skip = ShouldSkipValidation(request);
        if (skip)
        {
            await _next(context);
            return;
        }

        bool hasToken = TryGetToken(context, out string token);
        if (!hasToken)
        {
            await RespondWithUnauthorized(context);
            return;
        }

        try
        {
            using ApiResponse<AuthResponse> response = await _authClient.ValidateAsync(token.ToString(), context.RequestAborted);

            if (!response.IsSuccessStatusCode)
            {
                await RespondWithAuthServiceError(context, response);
                return;
            }

            context.Request.Headers.TryAdd("X-Credential-Id", response.Content!.Id.ToString());
        }
        catch (Exception)
        {
            await RespondWithInternalServerError(context);
            return;
        }

        await _next(context);
    }

    private bool ShouldSkipValidation(HttpRequest request)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(request.Path.Value);

        string serviceRequested = request.Path.Value?.Split('/')
            .Where(w => !string.IsNullOrEmpty(w))
            .FirstOrDefault() ?? string.Empty;

        return serviceRequested.Equals(_authServicesIdentifier, StringComparison.OrdinalIgnoreCase);
    }

    private bool TryGetToken(HttpContext context, out string token)
    {
        bool hasToken = context.Request.Headers.TryGetValue(_authHeaderIdentifier, out var tokenValues);
        token = hasToken ? tokenValues.ToString() : string.Empty;
        return !string.IsNullOrWhiteSpace(token);
    }

    private async Task RespondWithUnauthorized(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync(string.Empty);
    }

    private async Task RespondWithAuthServiceError(HttpContext context, ApiResponse<AuthResponse> response)
    {
        context.Response.StatusCode = (int)response.StatusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response.Content);
    }

    private async Task RespondWithInternalServerError(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Gateway Error",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        };
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}


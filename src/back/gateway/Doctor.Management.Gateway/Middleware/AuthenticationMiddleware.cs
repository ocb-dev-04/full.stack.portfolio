using Doctor.Management.Gateway.AuthClientServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Refit;
using System.Text.Json;

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
        ArgumentNullException.ThrowIfNullOrEmpty(request.Path.Value);

        string serviceRequested = request.Path.Value.Split('/')
            .Where(w => !string.IsNullOrEmpty(w))
            .First();
        if (serviceRequested.Equals(_authServicesIdentifier))
        {
            await _next(context);
            return;
        }

        bool hasToken = context.Request.Headers.TryGetValue(_authHeaderIdentifier, out var token);
        if (!hasToken || string.IsNullOrWhiteSpace(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(string.Empty);
            return;
        }

        try
        {
            using ApiResponse<AuthResponse> response = await _authClient.ValidateAsync(token.ToString(), context.RequestAborted);

            if (!response.IsSuccessStatusCode)
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response.Content));
                return;
            }

            context.Request.Headers.TryAdd("CredentialId", response.Content!.Id.ToString());
        }
        catch (Exception)
        {
            ProblemDetails problemDetails = new()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Gateway Error",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            };
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problemDetails);
            return;
        }

        // Continuar al siguiente middleware/ruta
        await _next(context);
    }
}


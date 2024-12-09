using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Services.Auth.Api.Extensions;

/// <summary>
/// Class to add healt checks
/// </summary>
internal static class HealthChecks
{
    /// <summary>
    /// Add all customs health checks
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    internal static void UseCustomHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/doctors/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                string result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    results = report.Entries.Select(e => new
                    {
                        key = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description ?? string.Empty,
                        duration = e.Value.Duration.ToString()
                    })
                });

                await context.Response.WriteAsync(result);
            }
        });
    }
}

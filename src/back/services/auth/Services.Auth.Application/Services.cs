using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Auth.Application;

public static class Services
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddFastEndpoints();

        return services;
    }
}

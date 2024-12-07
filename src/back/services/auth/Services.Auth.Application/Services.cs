using Microsoft.Extensions.DependencyInjection;
using Services.Auth.Application.Providers;

namespace Services.Auth.Application;

public static class Services
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<TokenProvider>();

        return services;
    }
}

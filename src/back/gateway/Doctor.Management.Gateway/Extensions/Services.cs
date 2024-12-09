using Consul;
using Microsoft.Extensions.Options;
using Doctor.Management.Gateway.Settings;
using Doctor.Management.Gateway.ProxyConfig;
using Yarp.ReverseProxy.Configuration;
using Refit;
using Doctor.Management.Gateway.AuthClient;

namespace Doctor.Management.Gateway.Extensions;

public static class Services
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services
            .AddConsulAsService()
            .AddAuthService()
            .AddReverseProxy();
    }

    private static IServiceCollection AddConsulAsService(this IServiceCollection services)
    {
        services.AddOptions<ConsulSettings>()
            .BindConfiguration(nameof(ConsulSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IConsulClient, ConsulClient>(sp => new ConsulClient(config =>
        {
            IOptions<ConsulSettings> settings = sp.GetRequiredService<IOptions<ConsulSettings>>();
            ArgumentNullException.ThrowIfNull(settings.Value, nameof(settings));

            config.Address = new Uri(settings.Value.Url);
            config.Token = settings.Value.Token;
        }));

        services.AddSingleton<InMemoryProxyConfig>();
        services.AddSingleton<ConsulProxyConfigProvider>();
        services.AddSingleton<IProxyConfigProvider, ConsulProxyConfigProvider>();

        return services;
    }

    private static IServiceCollection AddAuthService(this IServiceCollection services)
    {
        services.AddOptions<AuthSettings>()
            .BindConfiguration(nameof(AuthSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddRefitClient<IAuthClient>()
            .ConfigureHttpClient((sp, c) =>
                {
                    AuthSettings authSettings = sp.GetRequiredService<IOptions<AuthSettings>>().Value;
                    c.BaseAddress = new Uri(authSettings.Endpoint);
                });

        return services;
    }
}

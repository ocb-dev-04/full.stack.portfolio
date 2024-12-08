using Consul;
using Quartz;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Configuration;
using Doctor.Management.Gateway.Settings;
using Doctor.Management.Gateway.ProxyConfig;
using Doctor.Management.Gateway.JobsConfiguration;

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
            .AddUpdateRoutesJob()
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

        services.AddSingleton<IProxyConfigProvider, ConsulProxyConfigProvider>();

        return services;
    }

    private static IServiceCollection AddUpdateRoutesJob(this IServiceCollection services)
    {
        //services.AddQuartz(options 
        //    => options.UseMicrosoftDependencyInjectionJobFactory());
        services.AddQuartzHostedService(options 
            => options.WaitForJobsToComplete = true);

        services.ConfigureOptions<UpdateRoutesJobConfiguration>();

        return services;
    }
}

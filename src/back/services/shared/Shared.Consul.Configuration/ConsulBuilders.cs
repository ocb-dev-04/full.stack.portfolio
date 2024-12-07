using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Shared.Consul.Configuration.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Consul.Configuration;

public static class ConsulBuilders
{
    public static void AddServiceRegistry(this WebApplication app)
    {
        IOptions<ServiceRegistrationSettings> settings = app.Services.GetRequiredService<IOptions<ServiceRegistrationSettings>>();
        ArgumentNullException.ThrowIfNull(settings.Value, nameof(settings));

        app.AddServiceWhenStart(settings.Value);
        app.RemoveServiceWhenDown(settings.Value.Id);
    }

    private static void AddServiceWhenStart(this WebApplication app, ServiceRegistrationSettings settings)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            IConsulClient consulClient = app.Services.GetRequiredService<IConsulClient>();
            
            AgentServiceRegistration registration = new AgentServiceRegistration
            {
                ID = settings.Id,
                Name = settings.Name,
                Address = settings.Address,
                Port = settings.Port,
                Check = new AgentServiceCheck
                {
                    HTTP = settings.ServiceCheck.HealthEndpoint,
                    Interval = TimeSpan.FromSeconds(settings.ServiceCheck.IntervalToCheckInSeconds),
                    Timeout = TimeSpan.FromSeconds(settings.ServiceCheck.TimeoutCheckInSeconds)
                }
            };

            consulClient.Agent.ServiceRegister(registration).Wait();
        });
    }

    private static void RemoveServiceWhenDown(this WebApplication app, string id)
    {
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            IConsulClient consulClient = app.Services.GetRequiredService<IConsulClient>();
            consulClient.Agent.ServiceDeregister(id).Wait();
        });
    }
}

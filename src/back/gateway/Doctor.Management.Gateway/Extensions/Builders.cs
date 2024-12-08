using Doctor.Management.Gateway.ProxyConfig;

namespace Doctor.Management.Gateway.Extensions;

public static class Builders
{
    public static async Task AddDynamicRoutes(this WebApplication app)
    {
        ConsulProxyConfigProvider provider = app.Services.GetRequiredService<ConsulProxyConfigProvider>();
        
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(1));
        
        // let services start and configure information into consul services
        await Task.Delay(20 * 1000);
        await provider.UpdateRoutesAsync(cancellationTokenSource.Token);
    }
}

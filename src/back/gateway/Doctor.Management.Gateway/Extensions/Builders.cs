using Doctor.Management.Gateway.ProxyConfig;

namespace Doctor.Management.Gateway.Extensions;

public static class Builders
{
    public static async Task AddDynamicRoutes(this WebApplication app)
    {
		try
		{
            ConsulProxyConfigProvider provider = app.Services.GetRequiredService<ConsulProxyConfigProvider>();

            // We wait about 20 seconds for all services to be up
            await Task.Delay(20 * 1000);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));

            await provider.UpdateRoutesAsync(cancellationTokenSource.Token);
        }
		catch (Exception ex)
		{
            app.Logger.LogError("Some error ocurred: {0}", ex.Message);
		}
    }
}

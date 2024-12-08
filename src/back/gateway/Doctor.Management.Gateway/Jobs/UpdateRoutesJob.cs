using Quartz;
using Doctor.Management.Gateway.ProxyConfig;

namespace Doctor.Management.Gateway.Jobs;

public sealed class UpdateRoutesJob : IJob
{
    private readonly ConsulProxyConfigProvider _proxyConfigProvider;
    private readonly ILogger<UpdateRoutesJob> _logger;

    public UpdateRoutesJob(
        ConsulProxyConfigProvider proxyConfigProvider, 
        ILogger<UpdateRoutesJob> logger)
    {
        ArgumentNullException.ThrowIfNull(proxyConfigProvider, nameof(proxyConfigProvider));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _proxyConfigProvider = proxyConfigProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _proxyConfigProvider.UpdateRoutesAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Quartz Job: Error updating routes.");
        }
    }
}
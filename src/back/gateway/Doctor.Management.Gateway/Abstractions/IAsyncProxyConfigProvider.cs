using Yarp.ReverseProxy.Configuration;

namespace Doctor.Management.Gateway.Abstractions;

public interface IAsyncProxyConfigProvider
{
    Task<IProxyConfig> GetConfigAsync(CancellationToken cancellationToken);
}

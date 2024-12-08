
using Yarp.ReverseProxy.Configuration;
using Doctor.Management.Gateway.ProxyConfig;

using AgentService = Consul.AgentService;
using IConsulClient = Consul.IConsulClient;

namespace Doctor.Management.Gateway.Services;

public sealed class ConsulProxyConfigProvider : IProxyConfigProvider
{
    private readonly IConsulClient _consulClient;
    private readonly InMemoryProxyConfig _config;

    public ConsulProxyConfigProvider(IConsulClient consulClient)
    {
        _consulClient = consulClient;
        _config = new InMemoryProxyConfig(new List<RouteConfig>(), new List<ClusterConfig>());
    }

    public IProxyConfig GetConfig() => _config;

    public async Task UpdateRoutesAsync()
    {
        Consul.QueryResult<Dictionary<string, AgentService>>? services = await _consulClient.Agent.Services();
        List<RouteConfig> routes = services.Response.Select(s => new RouteConfig
        {
            RouteId = $"{s.Value.Service}-route",
            ClusterId = $"{s.Value.Service}-cluster",
            Match = new RouteMatch { Path = $"/{s.Value.Service}/{{**catch-all}}" }
        }).ToList();

        List<ClusterConfig> clusters = services.Response.Select(s => new ClusterConfig
        {
            ClusterId = $"{s.Value.Service}-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                {
                    $"{s.Value.Service}-destination",
                    new DestinationConfig 
                    { 
                        Address = new UriBuilder
                        {
                            Scheme = "http",
                            Host = s.Value.Address,
                            Port = s.Value.Port
                        }.Uri.ToString()
                    }
                }
            }
        }).ToList();

        _config.Update(routes, clusters);
    }
}

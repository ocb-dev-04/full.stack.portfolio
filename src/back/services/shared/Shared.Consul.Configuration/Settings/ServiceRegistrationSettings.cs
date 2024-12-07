using System.ComponentModel.DataAnnotations;

namespace Shared.Consul.Configuration.Settings;

public sealed class ServiceRegistrationSettings
{
    [Required]
    public string Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public int Port { get; set; }

    public ServiceCheck ServiceCheck { get; set; }
}

public sealed class ServiceCheck
{
    [Required]
    public string HealthEndpoint { get; set; }
    public int IntervalToCheckInSeconds { get; set; } = 10;
    public int TimeoutCheckInSeconds { get; set; } = 5;
}

using System.ComponentModel.DataAnnotations;
using Backbone.Infrastructure.EventBus;
using DevicesConfiguration = Backbone.Modules.Devices.ConsumerApi.Configuration;

namespace Backbone.SseServer;

public class Configuration
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();

    [Required]
    public ModulesConfiguration Modules { get; set; } = new();

    public class AuthenticationConfiguration
    {
        public string JwtSigningCertificate { get; set; } = "";
    }

    public class InfrastructureConfiguration
    {
        [Required]
        public EventBusConfiguration EventBus { get; set; } = new();
    }

    public class ModulesConfiguration
    {
        [Required]
        public DevicesConfiguration Devices { get; set; } = new();
    }
}

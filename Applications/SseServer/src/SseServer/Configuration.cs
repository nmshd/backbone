using System.ComponentModel.DataAnnotations;
using Backbone.Infrastructure.EventBus;
using DevicesConfiguration = Backbone.Modules.Devices.Module.Configuration;

namespace Backbone.SseServer;

public class Configuration
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public CorsConfiguration Cors { get; set; } = new();

    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();

    [Required]
    public ModulesConfiguration Modules { get; set; } = new();

    public class AuthenticationConfiguration
    {
        public string JwtSigningCertificate { get; set; } = "";
    }

    public class CorsConfiguration
    {
        public string AllowedOrigins { get; set; } = "";
        public string ExposedHeaders { get; set; } = "";
    }

    public class InfrastructureConfiguration
    {
        [Required]
        public EventBusOptions EventBus { get; set; } = new();
    }

    public class ModulesConfiguration
    {
        [Required]
        public DevicesConfiguration Devices { get; set; } = new();
    }
}

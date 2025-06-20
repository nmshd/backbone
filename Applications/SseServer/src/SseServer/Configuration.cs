using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.SseServer;

public class Configuration
{
    [Required]
    public required AuthenticationConfiguration Authentication { get; set; }

    public required CorsConfiguration Cors { get; set; }

    [Required]
    public required InfrastructureConfiguration Infrastructure { get; set; }

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
        public required EventBusConfiguration EventBus { get; set; }
    }
}

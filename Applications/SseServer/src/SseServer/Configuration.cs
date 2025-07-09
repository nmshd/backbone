using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.SseServer;

public class Configuration
{
    [Required]
    public required AuthenticationConfiguration Authentication { get; init; }

    public CorsConfiguration? Cors { get; set; }

    [Required]
    public required InfrastructureConfiguration Infrastructure { get; init; }

    [Required]
    public required SseServerConfiguration SseServer { get; set; }

    public class AuthenticationConfiguration
    {
        [Required]
        public required string JwtSigningCertificate { get; set; } = "";
    }

    public class CorsConfiguration
    {
        [Required]
        public string AllowedOrigins { get; set; } = "";

        [Required]
        public string ExposedHeaders { get; set; } = "";
    }

    public class InfrastructureConfiguration
    {
        [Required]
        public required EventBusConfiguration EventBus { get; set; }
    }

    public class SseServerConfiguration
    {
        [Required]
        public required int KeepAliveEventIntervalInSeconds { get; set; }
    }
}

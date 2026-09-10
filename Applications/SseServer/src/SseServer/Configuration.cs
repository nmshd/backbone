using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.SseServer;

public class Configuration
{
    [Required]
    public required AuthenticationConfiguration Authentication { get; init; }

    [Required]
    public SwaggerUiConfiguration SwaggerUi { get; init; } = new();

    public CorsConfiguration? Cors { get; init; }

    [Required]
    public required InfrastructureConfiguration Infrastructure { get; init; }

    [Required]
    public required SseServerConfiguration SseServer { get; init; }

    public class AuthenticationConfiguration
    {
        [Required]
        public required string JwtSigningCertificate { get; init; }
    }

    public class CorsConfiguration
    {
        [Required(AllowEmptyStrings = true)]
        public string AllowedOrigins { get; init; } = "";

        [Required(AllowEmptyStrings = true)]
        public string ExposedHeaders { get; init; } = "";
    }

    public class InfrastructureConfiguration
    {
        [Required]
        public required EventBusConfiguration EventBus { get; init; }
    }

    public class SseServerConfiguration
    {
        [Required]
        public required int KeepAliveEventIntervalInSeconds { get; init; }
    }
}

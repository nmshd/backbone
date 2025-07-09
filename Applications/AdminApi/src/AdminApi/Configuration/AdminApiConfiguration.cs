using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.AdminApi.Configuration;

public class AdminApiConfiguration
{
    [Required]
    public required AuthenticationConfiguration Authentication { get; init; }

    public CorsConfiguration? Cors { get; init; }

    [Required]
    public required InfrastructureConfiguration Infrastructure { get; init; }

    public class AuthenticationConfiguration
    {
        [Required]
        public required string ApiKey { get; init; }
    }

    public class CorsConfiguration
    {
        [Required]
        public string AllowedOrigins { get; init; } = string.Empty;

        [Required]
        public string ExposedHeaders { get; init; } = string.Empty;

        public bool AccessControlAllowCredentials { get; init; } = false;
    }

    public class InfrastructureConfiguration
    {
        [Required]
        public required EventBusConfiguration EventBus { get; init; }

        [Required]
        public required DatabaseConfiguration SqlDatabase { get; init; }
    }
}

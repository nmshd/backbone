using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.AdminApi.Configuration;

public class AdminApiConfiguration
{
    [Required]
    public required AuthenticationConfiguration Authentication { get; init; }

    [Required]
    public SwaggerUiConfiguration SwaggerUi { get; init; } = new();

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
        [Required(AllowEmptyStrings = true)]
        public string AllowedOrigins { get; init; } = "";

        [Required(AllowEmptyStrings = true)]
        public string ExposedHeaders { get; init; } = "";

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

using System.ComponentModel.DataAnnotations;
using Backbone.AdminApi.Infrastructure.Persistence;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.AdminApi.Configuration;

public class AdminApiConfiguration
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public CorsConfiguration Cors { get; set; } = new();

    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();

    public class AuthenticationConfiguration
    {
        [Required]
        public string ApiKey { get; set; } = string.Empty;
    }

    public class CorsConfiguration
    {
        public string AllowedOrigins { get; set; } = string.Empty;
        public string ExposedHeaders { get; set; } = string.Empty;
        public bool AccessControlAllowCredentials { get; set; } = false;
    }

    public class InfrastructureConfiguration
    {
        [Required]
        public EventBusOptions EventBus { get; set; } = new();

        [Required]
        public SqlDatabaseConfiguration SqlDatabase { get; set; } = new();
    }
}

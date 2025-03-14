using System.ComponentModel.DataAnnotations;
using Backbone.AdminApi.Infrastructure.Persistence;
using Backbone.Infrastructure.EventBus;

namespace Backbone.AdminApi.Configuration;

public class AdminConfiguration
{
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public CorsConfiguration Cors { get; set; } = new();

    [Required]
    public AdminInfrastructureConfiguration Infrastructure { get; set; } = new();

    public class AuthenticationConfiguration
    {
        public string ApiKey { get; set; } = string.Empty;
    }

    public class CorsConfiguration
    {
        public string AllowedOrigins { get; set; } = string.Empty;
        public string ExposedHeaders { get; set; } = string.Empty;
        public bool AccessControlAllowCredentials { get; set; } = false;
    }

    public class AdminInfrastructureConfiguration
    {
        [Required]
        public EventBusOptions EventBus { get; set; } = new();

        public SqlDatabaseConfiguration SqlDatabase { get; set; } = new();
    }
}

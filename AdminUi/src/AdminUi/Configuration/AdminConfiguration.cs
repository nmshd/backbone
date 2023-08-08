using System.ComponentModel.DataAnnotations;
using AdminUi.Infrastructure.Persistence;
using Backbone.Infrastructure.EventBus;

namespace AdminUi.Configuration;

public class AdminConfiguration
{
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public CorsConfiguration Cors { get; set; } = new();

    public SwaggerUiConfiguration SwaggerUi { get; set; } = new();

    [Required]
    public AdminInfrastructureConfiguration Infrastructure { get; set; } = new();

    [Required]
    public ModulesConfiguration Modules { get; set; } = new();

    public class AuthenticationConfiguration
    {
        public string ApiKey { get; set; } = string.Empty;
    }

    public class CorsConfiguration
    {
        public string AllowedOrigins { get; set; } = string.Empty;
        public string ExposedHeaders { get; set; } = string.Empty;
    }

    public class SwaggerUiConfiguration
    {
        [Required]
        public string TokenUrl { get; set; } = "";
    }

    public class AdminInfrastructureConfiguration
    {
        [Required]
        public EventBusConfiguration EventBus { get; set; } = new();
        public SqlDatabaseConfiguration SqlDatabase { get; set; } = new();
    }

    public class ModulesConfiguration
    {

        [Required]
        public DevicesConfiguration Devices { get; set; } = new();

        [Required]
        public QuotasConfiguration Quotas { get; set; } = new();
    }
}

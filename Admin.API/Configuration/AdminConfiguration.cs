using System.ComponentModel.DataAnnotations;
using Backbone.Infrastructure.EventBus;

namespace Admin.API.Configuration;

public class AdminConfiguration
{
    public CorsConfiguration Cors { get; set; } = new();

    public SwaggerUiConfiguration SwaggerUi { get; set; } = new();

    [Required]
    public ModulesConfiguration Modules { get; set; } = new();

    public class CorsConfiguration
    {
        public string AllowedOrigins { get; set; } = "";
        public string ExposedHeaders { get; set; } = "";
    }

    public class SwaggerUiConfiguration
    {
        [Required]
        public string TokenUrl { get; set; } = "";
    }

    public class ModulesConfiguration
    {

        [Required]
        public DevicesConfiguration Devices { get; set; } = new();
    }

    [Required]
    public AdminInfrastructureConfiguration Infrastructure { get; set; } = new();

    public class AdminInfrastructureConfiguration
    {
        [Required]
        public EventBusConfiguration EventBus { get; set; } = new();
    }
}
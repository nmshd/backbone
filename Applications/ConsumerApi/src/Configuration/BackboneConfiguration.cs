using System.ComponentModel.DataAnnotations;
using Backbone.Infrastructure.EventBus;

namespace Backbone.ConsumerApi.Configuration;

public class BackboneConfiguration
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public CorsConfiguration Cors { get; set; } = new();

    [Required]
    public BackboneInfrastructureConfiguration Infrastructure { get; set; } = new();

    public class AuthenticationConfiguration
    {
        [Required]
        public string JwtSigningCertificate { get; set; } = "";

        [Required]
        [Range(60, 3600)]
        public int JwtLifetimeInSeconds { get; set; }
    }

    public class CorsConfiguration
    {
        public string AllowedOrigins { get; set; } = "";
        public string ExposedHeaders { get; set; } = "";
    }

    public class BackboneInfrastructureConfiguration
    {
        [Required]
        public EventBusOptions EventBus { get; set; } = new();
    }
}

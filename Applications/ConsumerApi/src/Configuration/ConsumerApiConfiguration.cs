using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.ConsumerApi.Configuration;

public class ConsumerApiConfiguration
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public CorsConfiguration Cors { get; set; } = new();

    [Required]
    public ConsumerApiInfrastructureConfiguration Infrastructure { get; set; } = new();

    public OnboardingConfiguration Onboarding { get; set; } = new();

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

    public class ConsumerApiInfrastructureConfiguration
    {
        [Required]
        public EventBusConfiguration EventBus { get; set; } = new();
    }

    public class OnboardingConfiguration
    {
        public string AndroidAppUrl { get; set; } = null!;

        public string IosAppUrl { get; set; } = null!;
    }
}

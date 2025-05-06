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

    [Required]
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
        public App[] Apps { get; set; } = [];
    }

    public class App
    {
        [Required]
        public string Identifier { get; set; } = null!;

        [Required]
        public string DisplayName { get; set; } = null!;

        public Platform Ios { get; set; } = new();

        public Platform Android { get; set; } = new();

        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
        public string? PrimaryColor { get; set; }

        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
        public string? SecondaryColor { get; set; }

        public string? IconUrl { get; set; }
    }

    public class Platform
    {
        public string Url { get; set; } = null!;
    }
}

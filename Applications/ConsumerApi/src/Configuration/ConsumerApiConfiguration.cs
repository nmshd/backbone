using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Microsoft.AspNetCore.WebUtilities;

namespace Backbone.ConsumerApi.Configuration;

public class ConsumerApiConfiguration
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public CorsConfiguration Cors { get; set; } = new();

    [Required]
    public ConsumerApiInfrastructureConfiguration Infrastructure { get; set; } = new();

    [Required]
    public AppOnboardingConfiguration AppOnboarding { get; set; } = new();

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

    public class AppOnboardingConfiguration : IValidatableObject
    {
        public App[] Apps { get; set; } = [];

        public string? DefaultAppId { get; set; }

        public class App
        {
            private const string IPHONE_DEVICE_HINT = "iphone";
            private const string MAC_OS_DEVICE_HINT = "ipad";

            [Required]
            public string Id { get; set; } = null!;

            [Required]
            public string DisplayName { get; set; } = null!;

            public Platform? Ios { get; set; } = new();

            public Platform? Android { get; set; } = new();

            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string? PrimaryColor { get; set; }

            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string? SecondaryColor { get; set; }

            public string? IconUrl { get; set; }

            public Dictionary<PlatformType, string> GetAllConfiguredAppStoreLinks()
            {
                var appStoreLinks = new Dictionary<PlatformType, string>();

                if (Ios != null)
                {
                    appStoreLinks.Add(PlatformType.Ios, QueryHelpers.AddQueryString(Ios.Url, "platform", IPHONE_DEVICE_HINT));
                    appStoreLinks.Add(PlatformType.Macos, QueryHelpers.AddQueryString(Ios.Url, "platform", MAC_OS_DEVICE_HINT));
                }

                if (Android != null)
                    appStoreLinks.Add(PlatformType.Android, Android.Url);

                return appStoreLinks;
            }


            public enum PlatformType
            {
                Android,
                Ios,
                Macos,
                Unknown
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DefaultAppId != null && Apps.All(a => a.Id != DefaultAppId!))
                yield return new ValidationResult($"The {nameof(DefaultAppId)} currently set to \"{DefaultAppId}\" is not part of the configured apps in the {nameof(AppOnboardingConfiguration)}.",
                    [nameof(AppOnboardingConfiguration), nameof(DefaultAppId)]);
        }
    }

    public class Platform
    {
        public string Url { get; set; } = null!;
    }
}

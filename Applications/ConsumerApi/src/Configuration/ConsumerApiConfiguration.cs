using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Microsoft.AspNetCore.WebUtilities;

namespace Backbone.ConsumerApi.Configuration;

public class ConsumerApiConfiguration : IValidatableObject
{
    [Required]
    public AuthenticationConfiguration Authentication { get; set; } = new();

    public CorsConfiguration Cors { get; set; } = new();

    [Required]
    public ConsumerApiInfrastructureConfiguration Infrastructure { get; set; } = new();

    public string? DefaultAppId { get; set; }

    public List<AppConfig> Apps { get; set; } = [];

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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DefaultAppId != null && Apps.All(a => a.Id != DefaultAppId))
            yield return new ValidationResult($"The {nameof(DefaultAppId)} currently set to \"{DefaultAppId}\" is not part of the configured apps.",
                [nameof(Apps), nameof(DefaultAppId)]);
    }
}

public class AppConfig
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string DisplayName { get; set; } = null!;

    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
    public string? PrimaryColor { get; set; }

    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
    public string? SecondaryColor { get; set; }

    public string? IconUrl { get; set; }

    public AndroidConfig? Android { get; set; }
    public IosConfig? Ios { get; set; }

    public Dictionary<PlatformType, string> GetAllConfiguredAppStoreLinks()
    {
        const string iphoneDeviceHint = "iphone";
        const string macOsDeviceHint = "ipad";

        var appStoreLinks = new Dictionary<PlatformType, string>();

        if (Ios != null)
        {
            appStoreLinks.Add(PlatformType.Ios, QueryHelpers.AddQueryString(Ios.AppStoreUrl, "platform", iphoneDeviceHint));
            appStoreLinks.Add(PlatformType.Macos, QueryHelpers.AddQueryString(Ios.AppStoreUrl, "platform", macOsDeviceHint));
        }

        if (Android != null)
            appStoreLinks.Add(PlatformType.Android, Android.PlayStoreUrl);

        return appStoreLinks;
    }


    public enum PlatformType
    {
        Android,
        Ios,
        Macos,
        Unknown
    }

    public class AndroidConfig
    {
        [Required]
        public string Identifier { get; set; } = null!;

        [Required]
        public List<string> Sha256CertFingerprints { get; set; } = null!;

        [Required]
        public string PlayStoreUrl { get; set; } = null!;

        [Required]
        public PushNotificationsConfig PushNotifications { get; set; } = null!;

        public class PushNotificationsConfig
        {
            [Required]
            public string ServiceAccountJson { get; set; } = null!;
        }
    }

    public class IosConfig
    {
        [Required]
        public string TeamId { get; set; } = null!;

        [Required]
        public PushNotificationsConfig PushNotifications { get; set; } = null!;

        [Required]
        public string ApplicationIdentifierPrefix { get; set; } = null!;

        [Required]
        public string BundleId { get; set; } = null!;

        [Required]
        public string AppStoreUrl { get; set; } = null!;

        public class PushNotificationsConfig
        {
            [Required]
            public string KeyId { get; set; } = null!;

            [Required]
            public string PrivateKey { get; set; } = null!;
        }
    }
}

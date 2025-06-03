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

    public AppOnboardingConfiguration? AppOnboarding { get; set; } = new();

    public WellKnownEndpointsConfiguration WellKnownEndpoints { get; set; } = new();

    public class WellKnownEndpointsConfiguration
    {
        public string[] AppleAppSiteAssociations { get; set; } = [];

        public AndroidAssetLink[] AndroidAssetLinks { get; set; } = [];

        public class AndroidAssetLink
        {
            public string PackageName { get; set; } = "";
            public string[] Sha256CertFingerprints { get; set; } = [];
        }
    }

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
            [Required]
            public string Id { get; set; } = null!;

            [Required]
            public string DisplayName { get; set; } = null!;

            [Required]
            public StoreConfig AppleAppStore { get; set; } = new();

            [Required]
            public StoreConfig GooglePlayStore { get; set; } = new();

            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string BackgroundColor { get; set; } = "#FFFFFF";

            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string PrimaryColor { get; set; } = "#000000";

            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string SecondaryColor { get; set; } = "#000000";

            [Required]
            [RegularExpression("^(https?:\\/\\/[^\\s]+|data:image\\/[a-zA-Z+]+;base64,[A-Za-z0-9+\\/=]+)$",
                ErrorMessage = "Invalid URL. Must be either an http(s) URL that points to an image or a data url with the image as base64 encoded content (e.g. data:image/png;base64,iVBO...).")]
            public string BannerUrl { get; set; } = null!;

            [Required]
            [RegularExpression("^(https?:\\/\\/[^\\s]+|data:image\\/[a-zA-Z+]+;base64,[A-Za-z0-9+\\/=]+)$",
                ErrorMessage = "Invalid URL. Must be either an http(s) URL that points to an image or a data url with the image as base64 encoded content (e.g. data:image/png;base64,iVBO...).")]
            public string IconUrl { get; set; } = null!;

            public class StoreConfig
            {
                public string? AppLink { get; set; } = null!;
                public string NoLinkText { get; set; } = "This app is not officially available in this store yet. Please check back later.";
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DefaultAppId != null && Apps.All(a => a.Id != DefaultAppId))
                yield return new ValidationResult($"The {nameof(DefaultAppId)} currently set to \"{DefaultAppId}\" is not part of the configured apps.",
                    [nameof(Apps), nameof(DefaultAppId)]);
        }
    }
}

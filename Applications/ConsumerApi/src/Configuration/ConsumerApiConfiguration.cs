using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.ConsumerApi.Configuration;

public class ConsumerApiConfiguration
{
    [Required]
    public required AuthenticationConfiguration Authentication { get; init; }

    public CorsConfiguration? Cors { get; set; }

    [Required]
    public required ConsumerApiInfrastructureConfiguration Infrastructure { get; init; }

    public AppOnboardingConfiguration? AppOnboarding { get; set; }

    public WellKnownEndpointsConfiguration? WellKnownEndpoints { get; set; }

    public class WellKnownEndpointsConfiguration
    {
        [Required]
        public required string[] AppleAppSiteAssociations { get; set; }

        [Required]
        public required AndroidAssetLink[] AndroidAssetLinks { get; set; }

        public class AndroidAssetLink
        {
            [Required]
            public required string PackageName { get; set; }

            [Required]
            public required string[] Sha256CertFingerprints { get; set; }
        }
    }

    public class AuthenticationConfiguration
    {
        [Required]
        public required string JwtSigningCertificate { get; set; }

        [Required]
        [Range(60, 3600)]
        public required int JwtLifetimeInSeconds { get; set; }
    }

    public class CorsConfiguration
    {
        [Required]
        public string AllowedOrigins { get; set; } = "";

        [Required]
        public string ExposedHeaders { get; set; } = "";
    }

    public class ConsumerApiInfrastructureConfiguration
    {
        [Required]
        public required EventBusConfiguration EventBus { get; set; }
    }

    public class AppOnboardingConfiguration : IValidatableObject
    {
        [Required]
        public required App[] Apps { get; set; }

        public string? DefaultAppId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DefaultAppId != null && Apps.All(a => a.Id != DefaultAppId))
                yield return new ValidationResult($"The {nameof(DefaultAppId)} currently set to \"{DefaultAppId}\" is not part of the configured apps.",
                    [nameof(Apps), nameof(DefaultAppId)]);
        }

        public class App
        {
            [Required]
            public required string Id { get; set; }

            [Required]
            public required string DisplayName { get; set; }

            [Required]
            public required string Description { get; set; }

            [Required]
            public required StoreConfig AppleAppStore { get; set; }

            [Required]
            public required StoreConfig GooglePlayStore { get; set; }

            [Required]
            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string BackgroundColor { get; set; } = "#FFFFFF";

            [Required]
            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string PrimaryColor { get; set; } = "#000000";

            [Required]
            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string SecondaryColor { get; set; } = "#000000";

            [Required]
            [RegularExpression("^(https?:\\/\\/[^\\s]+|data:image\\/[a-zA-Z+]+;base64,[A-Za-z0-9+\\/=]+)$",
                ErrorMessage = "Invalid URL. Must be either an http(s) URL that points to an image or a data url with the image as base64 encoded content (e.g. data:image/png;base64,iVBO...).")]
            public required string BannerUrl { get; set; }

            [Required]
            [RegularExpression("^(https?:\\/\\/[^\\s]+|data:image\\/[a-zA-Z+]+;base64,[A-Za-z0-9+\\/=]+)$",
                ErrorMessage = "Invalid URL. Must be either an http(s) URL that points to an image or a data url with the image as base64 encoded content (e.g. data:image/png;base64,iVBO...).")]
            public required string IconUrl { get; set; }

            public class StoreConfig
            {
                public string? AppLink { get; set; } = null!;

                [Required]
                public required string NoLinkText { get; set; } = "This app is not officially available in this store yet. Please check back later.";
            }
        }
    }
}

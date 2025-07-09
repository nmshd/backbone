using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.ConsumerApi.Configuration;

public class ConsumerApiConfiguration
{
    [Required]
    public required AuthenticationConfiguration Authentication { get; init; }

    public CorsConfiguration? Cors { get; init; }

    [Required]
    public required ConsumerApiInfrastructureConfiguration Infrastructure { get; init; }

    public AppOnboardingConfiguration? AppOnboarding { get; init; }

    public WellKnownEndpointsConfiguration? WellKnownEndpoints { get; init; }

    public class WellKnownEndpointsConfiguration
    {
        [Required]
        public required string[] AppleAppSiteAssociations { get; init; }

        [Required]
        public required AndroidAssetLink[] AndroidAssetLinks { get; init; }

        public class AndroidAssetLink
        {
            [Required]
            public required string PackageName { get; init; }

            [Required]
            public required string[] Sha256CertFingerprints { get; init; }
        }
    }

    public class AuthenticationConfiguration
    {
        [Required]
        public required string JwtSigningCertificate { get; init; }

        [Required]
        [Range(60, 3600)]
        public required int JwtLifetimeInSeconds { get; init; }
    }

    public class CorsConfiguration
    {
        [Required(AllowEmptyStrings = true)]
        public string AllowedOrigins { get; init; } = "";

        [Required(AllowEmptyStrings = true)]
        public string ExposedHeaders { get; init; } = "";
    }

    public class ConsumerApiInfrastructureConfiguration
    {
        [Required]
        public required EventBusConfiguration EventBus { get; init; }
    }

    public class AppOnboardingConfiguration : IValidatableObject
    {
        [Required]
        public required App[] Apps { get; init; }

        public string? DefaultAppId { get; init; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DefaultAppId != null && Apps.All(a => a.Id != DefaultAppId))
                yield return new ValidationResult($"The {nameof(DefaultAppId)} currently set to \"{DefaultAppId}\" is not part of the configured apps.",
                    [nameof(Apps), nameof(DefaultAppId)]);
        }

        public class App
        {
            [Required]
            public required string Id { get; init; }

            [Required]
            public required string DisplayName { get; init; }

            [Required]
            public required string Description { get; init; }

            [Required]
            public required StoreConfig AppleAppStore { get; init; }

            [Required]
            public required StoreConfig GooglePlayStore { get; init; }

            [Required]
            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string BackgroundColor { get; init; } = "#FFFFFF";

            [Required]
            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string PrimaryColor { get; init; } = "#000000";

            [Required]
            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string SecondaryColor { get; init; } = "#000000";

            [Required]
            [RegularExpression("^(https?:\\/\\/[^\\s]+|data:image\\/[a-zA-Z+]+;base64,[A-Za-z0-9+\\/=]+)$",
                ErrorMessage = "Invalid URL. Must be either an http(s) URL that points to an image or a data url with the image as base64 encoded content (e.g. data:image/png;base64,iVBO...).")]
            public required string BannerUrl { get; init; }

            [Required]
            [RegularExpression("^(https?:\\/\\/[^\\s]+|data:image\\/[a-zA-Z+]+;base64,[A-Za-z0-9+\\/=]+)$",
                ErrorMessage = "Invalid URL. Must be either an http(s) URL that points to an image or a data url with the image as base64 encoded content (e.g. data:image/png;base64,iVBO...).")]
            public required string IconUrl { get; init; }

            public class StoreConfig
            {
                public string? AppLink { get; init; } = null!;

                [Required]
                public string NoLinkText { get; init; } = "This app is not officially available in this store yet. Please check back later.";
            }
        }
    }
}

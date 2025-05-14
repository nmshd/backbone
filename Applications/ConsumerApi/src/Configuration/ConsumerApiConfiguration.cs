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

            public StoreConfig? AppleAppStore { get; set; }

            public StoreConfig? GooglePlayStore { get; set; }

            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string? PrimaryColor { get; set; }

            [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use a hex color code like #FFFFFF.")]
            public string? SecondaryColor { get; set; }

            public string? IconUrl { get; set; }

            public Dictionary<StoreType, string> GetAllConfiguredStoreLinks()
            {
                var storeLinks = new Dictionary<StoreType, string>();

                if (AppleAppStore != null)
                    storeLinks.Add(StoreType.AppleAppStore, AppleAppStore.AppUrl);

                if (GooglePlayStore != null)
                    storeLinks.Add(StoreType.GooglePlayStore, GooglePlayStore.AppUrl);

                return storeLinks;
            }


            public enum StoreType
            {
                GooglePlayStore,
                AppleAppStore,
                Unknown
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DefaultAppId != null && Apps.All(a => a.Id != DefaultAppId))
                yield return new ValidationResult($"The {nameof(DefaultAppId)} currently set to \"{DefaultAppId}\" is not part of the configured apps.",
                    [nameof(Apps), nameof(DefaultAppId)]);
        }
    }

    public class StoreConfig
    {
        [Required]
        public string AppUrl { get; set; } = null!;
    }
}

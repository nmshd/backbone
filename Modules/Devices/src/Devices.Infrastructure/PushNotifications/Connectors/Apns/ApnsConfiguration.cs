using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;

public class ApnsConfiguration
{
    public bool Enabled { get; set; } = true;

    [RequiredIf(nameof(Enabled), true)]
    [MinLength(1)]
    public required Dictionary<string, Key> Keys { get; set; } = new();

    [RequiredIf(nameof(Enabled), true)]
    [MinLength(1)]
    public required Dictionary<string, Bundle> Bundles { get; set; } = new();

    public bool HasConfigForBundleId(string bundleId)
    {
        var bundle = Bundles.GetValueOrDefault(bundleId);
        return bundle != null && !bundle.KeyName.IsNullOrEmpty() && Keys.ContainsKey(bundle.KeyName) && !Keys[bundle.KeyName].PrivateKey.IsNullOrEmpty();
    }

    public Key GetKeyInformationForBundleId(string bundleId)
    {
        var bundle = Bundles.GetValueOrDefault(bundleId);
        return bundle == null ? throw new Exception($"No bundle configuration for bundle id '{bundleId}' was found.") : Keys[bundle.KeyName];
    }

    public Bundle GetBundleById(string bundleId)
    {
        return Bundles[bundleId];
    }

    public List<string> GetSupportedBundleIds()
    {
        return Bundles.Where(bundle => HasConfigForBundleId(bundle.Key)).Select(bundle => bundle.Key).ToList();
    }

    public class Bundle
    {
        [Required]
        [MinLength(1)]
        public required string KeyName { get; set; }
    }

    public class Key
    {
        [Required]
        [MinLength(1)]
        public required string TeamId { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public required string KeyId { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public required string PrivateKey { get; set; } = string.Empty;
    }
}

public class ApnsConfigurationValidator : IValidateOptions<ApnsConfiguration>
{
    private readonly DevicesDbContext _dbContext;

    public ApnsConfigurationValidator(DevicesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ValidateOptionsResult Validate(string? name, ApnsConfiguration configuration)
    {
        if (!configuration.Enabled)
            return ValidateOptionsResult.Success;

        var supportedBundleIds = configuration.GetSupportedBundleIds();
        var failingBundleIds = _dbContext.GetApnsBundleIdsForWhichNoConfigurationExists(supportedBundleIds).GetAwaiter().GetResult();

        if (failingBundleIds.IsEmpty())
            return ValidateOptionsResult.Success;

        var configuredBundleIdsString = string.Join(", ", supportedBundleIds.Select(x => $"'{x}'"));

        var details = "The questionable bundle ids are: " + string.Join(", ", failingBundleIds.Select(x => $"'{x}'")) + $". The configured bundle ids are: {configuredBundleIdsString}.";

        return ValidateOptionsResult.Fail($"There are APNs registrations in the database with a bundle id for which there is no configuration.\n{details}");
    }
}

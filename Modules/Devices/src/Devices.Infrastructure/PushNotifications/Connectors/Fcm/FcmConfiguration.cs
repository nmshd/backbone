using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;

public class FcmConfiguration
{
    [Required]
    public bool Enabled { get; set; } = true;

    [RequiredIf(nameof(Enabled), true)]
    [MinLength(1)]
    public required Dictionary<string, ServiceAccount> ServiceAccounts { get; set; } = [];

    [RequiredIf(nameof(Enabled), true)]
    [MinLength(1)]
    public required Dictionary<string, ServiceAccountInformation> Apps { get; set; } = [];

    public class ServiceAccountInformation
    {
        [Required]
        [MinLength(1)]
        public string ServiceAccountName { get; set; } = string.Empty;
    }

    public bool HasConfigForAppId(string appId)
    {
        var app = Apps.GetValueOrDefault(appId);

        if (app == null) return false;

        var serviceAccountForServiceAccountNameExists = ServiceAccounts.ContainsKey(app.ServiceAccountName);

        return serviceAccountForServiceAccountNameExists;
    }

    public string? GetServiceAccountForAppId(string appId)
    {
        var app = Apps.GetValueOrDefault(appId);

        if (app == null)
            return null;

        var serviceAccount = ServiceAccounts.GetValueOrDefault(app.ServiceAccountName);

        return serviceAccount?.ServiceAccountJson;
    }

    public List<string> GetSupportedAppIds()
    {
        return Apps.Where(app => HasConfigForAppId(app.Key)).Select(app => app.Key).ToList();
    }

    public class ServiceAccount
    {
        [Required]
        [MinLength(1)]
        public required string ServiceAccountJson { get; set; } = string.Empty;
    }
}

public class FcmConfigurationValidator : IValidateOptions<FcmConfiguration>
{
    private readonly DevicesDbContext _dbContext;

    public FcmConfigurationValidator(DevicesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ValidateOptionsResult Validate(string? name, FcmConfiguration configuration)
    {
        if (!configuration.Enabled)
            return ValidateOptionsResult.Success;

        var supportedAppIds = configuration.GetSupportedAppIds();
        var failingAppIds = _dbContext.GetFcmAppIdsForWhichNoConfigurationExists(supportedAppIds).GetAwaiter().GetResult();

        if (failingAppIds.IsEmpty())
            return ValidateOptionsResult.Success;

        var configuredAppIdsString = string.Join(", ", supportedAppIds.Select(x => $"'{x}'"));

        var details = "The questionable app ids are: " + string.Join(", ", failingAppIds.Select(x => $"'{x}'")) + $". The configured app ids are: {configuredAppIdsString}.";

        return ValidateOptionsResult.Fail($"There are FCM registrations in the database with an app id for which there is no configuration.\n{details}");
    }
}

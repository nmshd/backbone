using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;

public class FcmOptions
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

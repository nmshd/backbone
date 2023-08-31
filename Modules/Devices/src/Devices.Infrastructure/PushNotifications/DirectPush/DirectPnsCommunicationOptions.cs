using Enmeshed.Tooling.Extensions;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public class DirectPnsCommunicationOptions
{
    public FcmOptions Fcm { get; set; }

    public ApnsOptions Apns { get; set; }

    public class FcmOptions
    {
        public string DefaultAppId { get; set; }
        public Dictionary<string, ServiceAccount> ServiceAccounts { get; set; } = new();
        public class ServiceAccount
        {
            public string ServiceAccountJson { get; set; } = string.Empty;
        }
        public Dictionary<string, ServiceAccountInformation> Apps { get; set; } = new();
        public class ServiceAccountInformation
        {
            public string ServiceAccountName { get; set; } = string.Empty;
        }

        public bool HasConfigForAppId(string appId)
        {
            var app = Apps.GetValueOrDefault(appId);
            return app != null && !app.ServiceAccountName.IsNullOrEmpty() && ServiceAccounts.ContainsKey(app.ServiceAccountName) && !ServiceAccounts[app.ServiceAccountName].ServiceAccountJson.IsNullOrEmpty();
        }

        public string? GetServiceAccountForAppId(string appId)
        {
            var app = Apps.GetValueOrDefault(appId);
            return ServiceAccounts.GetValueOrDefault(app.ServiceAccountName)?.ServiceAccountJson;
        }

        public List<string> GetSupportedAppIds()
        {
            return Apps.Where(app => HasConfigForAppId(app.Key)).Select(app => app.Key).ToList();
        }
    }

    public class ApnsOptions
    {
        public string DefaultBundleId { get; set; }
        public Dictionary<string, Key> Keys { get; set; } = new();
        public class Key
        {
            public string TeamId { get; set; } = string.Empty;
            public string KeyId { get; set; } = string.Empty;
            public string PrivateKey { get; set; } = string.Empty;
        }
        public Dictionary<string, Bundle> Bundles { get; set; } = new();
        public class Bundle
        {
            public string KeyName { get; set; }
            public string Server => ServerType switch
            {
                ApnsServerType.Development => "https://api.development.push.apple.com:443/3/device/",
                ApnsServerType.Production => "https://api.push.apple.com:443/3/device/",
                _ => throw new ArgumentOutOfRangeException()
            };
            public ApnsServerType ServerType { get; set; }
            public enum ApnsServerType
            {
                Development,
                Production
            }
        }

        public bool HasConfigForBundleId(string bundleId)
        {
            var bundle = Bundles.GetValueOrDefault(bundleId);
            return bundle != null && !bundle.KeyName.IsNullOrEmpty() && Keys.ContainsKey(bundle.KeyName) && !Keys[bundle.KeyName].PrivateKey.IsNullOrEmpty();
        }

        public Key GetKeyInformationForBundleId(string bundleId)
        {
            return Keys[Bundles[bundleId].KeyName];
        }

        public Bundle GetBundleById(string bundleId)
        {
            return Bundles[bundleId];
        }

        public List<string> GetSupportedBundleIds()
        {
            return Bundles.Where(bundle => HasConfigForBundleId(bundle.Key)).Select(bundle => bundle.Key).ToList();
        }
    }
}

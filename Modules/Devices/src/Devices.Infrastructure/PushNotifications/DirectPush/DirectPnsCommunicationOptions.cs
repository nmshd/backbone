using System.ComponentModel.DataAnnotations;
using Enmeshed.Tooling.Extensions;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public class DirectPnsCommunicationOptions
{
    public FcmOptions Fcm { get; set; }

    public ApnsOptions Apns { get; set; }

    public class FcmOptions
    {
        [Required]
        [MinLength(1)]
        public Dictionary<string, ServiceAccount> ServiceAccounts { get; set; } = new();

        [Required]
        [MinLength(1)]
        public Dictionary<string, ServiceAccountInformation> Apps { get; set; } = new();

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

            if (serviceAccount == null)
                return null;

            return serviceAccount.ServiceAccountJson;
        }

        public List<string> GetSupportedAppIds()
        {
            return Apps.Where(app => HasConfigForAppId(app.Key)).Select(app => app.Key).ToList();
        }

        public class ServiceAccount
        {
            [Required]
            [MinLength(1)]
            public string ServiceAccountJson { get; set; } = string.Empty;
        }
    }

    public class ApnsOptions
    {
        [Required]
        [MinLength(1)]
        public Dictionary<string, Key> Keys { get; set; } = new();

        [Required]
        [MinLength(1)]
        public Dictionary<string, Bundle> Bundles { get; set; } = new();

        public bool HasConfigForBundleId(string bundleId)
        {
            var bundle = Bundles.GetValueOrDefault(bundleId);
            return bundle != null && !bundle.KeyName.IsNullOrEmpty() && Keys.ContainsKey(bundle.KeyName) && !Keys[bundle.KeyName].PrivateKey.IsNullOrEmpty();
        }

        public Key GetKeyInformationForBundleId(string bundleId)
        {
            var bundle = Bundles.GetValueOrDefault(bundleId);

            if (bundle == null)
                return null;

            return Keys[bundle.KeyName];
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
            public string KeyName { get; set; }

            public string Server => ServerType switch
            {
                ApnsServerType.Development => "https://api.development.push.apple.com:443/3/device/",
                ApnsServerType.Production => "https://api.push.apple.com:443/3/device/",
                _ => throw new ArgumentOutOfRangeException()
            };

            [Required]
            public ApnsServerType ServerType { get; set; }

            public enum ApnsServerType
            {
                Development,
                Production
            }
        }

        public class Key
        {
            [Required]
            [MinLength(1)]
            public string TeamId { get; set; } = string.Empty;

            [Required]
            [MinLength(1)]
            public string KeyId { get; set; } = string.Empty;

            [Required]
            [MinLength(1)]
            public string PrivateKey { get; set; } = string.Empty;
        }
    }
}

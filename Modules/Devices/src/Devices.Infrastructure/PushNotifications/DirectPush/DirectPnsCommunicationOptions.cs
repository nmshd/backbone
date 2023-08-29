namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public class DirectPnsCommunicationOptions
{
    public FcmOptions Fcm { get; set; }

    public ApnsOptions Apns { get; set; }

    public class FcmOptions
    {
        public string DefaultBundleId { get; set; }
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
    }
}

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public class DirectPnsCommunicationOptions
{
    public FcmOptions Fcm { get; set; }

    public ApnsOptions Apns { get; set; }

    public class FcmOptions
    {
        public Dictionary<string, ServiceAccounts> KeysByApplicationId { get; set; } = new();
        public class ServiceAccounts
        {
            public string ServiceAccountJson { get; set; } = string.Empty;
        }
    }

    public class ApnsOptions
    {
        public Dictionary<string, Key> KeysByBundleId { get; set; } = new();
        public class Key
        {
            public string TeamId { get; set; } = string.Empty;
            public string KeyId { get; set; } = string.Empty;
            public string PrivateKey { get; set; } = string.Empty;
            public string AppBundleIdentifier { get; set; } = string.Empty;
            public ApnsServerType ServerType { get; set; }
            public string Server => ServerType switch
            {
                ApnsServerType.Development => "https://api.development.push.apple.com:443/3/device/",
                ApnsServerType.Production => "https://api.push.apple.com:443/3/device/",
                _ => throw new ArgumentOutOfRangeException()
            };
            public enum ApnsServerType
            {
                Development,
                Production
            }
        }
    }
}

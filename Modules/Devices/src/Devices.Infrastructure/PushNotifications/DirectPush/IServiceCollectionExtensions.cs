using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public static class IServiceCollectionExtensions
{
    public static void AddDirectPushNotifications(this IServiceCollection services, DirectPnsCommunicationOptions? options)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = options?.Fcm?.ServiceAccountJson is null
                ? GoogleCredential.GetApplicationDefault()
                : GoogleCredential.FromJson(options.Fcm.ServiceAccountJson)
        });
        services.AddHttpClient();
        services.AddTransient<PnsConnectorFactory, PnsConnectorFactoryImpl>();
        services.AddTransient<FirebaseCloudMessagingConnector>();
        services.AddTransient<ApplePushNotificationServiceConnector>();
        services.AddTransient<IPushService, DirectPushService>();
        services.AddTransient<IJwtGenerator, JwtGenerator>();
        services.AddSingleton(FirebaseMessaging.DefaultInstance);
    }

    public class DirectPnsCommunicationOptions
    {
        public FcmOptions Fcm { get; set; }

        public class FcmOptions
        {
            public string ServiceAccountJson { get; set; } = string.Empty;
        }

        public ApnsOptions Apns { get; set; }

        public class ApnsOptions
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
        }

        public enum ApnsServerType
        {
            Development,
            Production
        }
    }
}

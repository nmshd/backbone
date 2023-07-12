using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
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

        services.AddTransient<PnsConnectorFactory, PnsConnectorFactoryImpl>();
        services.AddTransient<FirebaseCloudMessagingConnector>();
        services.AddTransient<IPushService, DirectPushService>();
        services.AddSingleton(FirebaseMessaging.DefaultInstance);
    }

    public class DirectPnsCommunicationOptions
    {
        public FcmOptions Fcm { get; set; }

        public class FcmOptions
        {
            public string ServiceAccountJson { get; set; } = string.Empty;
        }
    }
}

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
    public static void AddDirectPushNotifications(this IServiceCollection services, DirectPnsCommunicationOptions options)
    {
        services.AddTransient<PnsConnectorFactory, PnsConnectorFactoryImpl>();
        services.AddFcm(options.Fcm);
        services.AddApns();
    }

    private static void AddFcm(this IServiceCollection services, DirectPnsCommunicationOptions.FcmOptions options)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = options?.ServiceAccountJson is null
                ? GoogleCredential.GetApplicationDefault()
                : GoogleCredential.FromJson(options.ServiceAccountJson)
        });
        services.AddTransient<FirebaseCloudMessagingConnector>();
        services.AddSingleton(FirebaseMessaging.DefaultInstance);
    }

    private static void AddApns(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTransient<ApplePushNotificationServiceConnector>();
        services.AddTransient<IPushService, DirectPushService>();
        services.AddTransient<IJwtGenerator, JwtGenerator>();
        services.AddSingleton<ApnsJwtCache>();
    }
}

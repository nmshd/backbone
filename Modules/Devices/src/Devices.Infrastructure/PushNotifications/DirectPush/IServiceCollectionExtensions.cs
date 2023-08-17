using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public static class IServiceCollectionExtensions
{
    public static void AddDirectPushNotifications(this IServiceCollection services, DirectPnsCommunicationOptions options)
    {
        services.AddTransient<PnsConnectorFactory, PnsConnectorFactoryImpl>();
        services.AddFcm();
        services.AddApns();
    }

    private static void AddFcm(this IServiceCollection services)
    {
        services.AddSingleton<FirebaseMessagingFactory>();
        services.AddTransient<FirebaseCloudMessagingConnector>();
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

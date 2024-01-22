using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public static class IServiceCollectionExtensions
{
    public static void AddDirectPushNotifications(this IServiceCollection services, DirectPnsCommunicationOptions options)
    {
        if (options.Apns != null) 
            services.AddSingleton<IOptions<DirectPnsCommunicationOptions.ApnsOptions>>(new OptionsWrapper<DirectPnsCommunicationOptions.ApnsOptions>(options.Apns));

        if (options.Fcm != null) 
            services.AddSingleton<IOptions<DirectPnsCommunicationOptions.FcmOptions>>(new OptionsWrapper<DirectPnsCommunicationOptions.FcmOptions>(options.Fcm));

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
        services.AddTransient<IPushNotificationRegistrationService, DirectPushService>();
        services.AddTransient<IPushNotificationSender, DirectPushService>();
        services.AddTransient<IJwtGenerator, JwtGenerator>();
        services.AddSingleton<ApnsJwtCache>();
    }
}

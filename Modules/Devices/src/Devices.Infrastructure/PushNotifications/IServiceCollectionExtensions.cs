using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Dummy;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public static class IServiceCollectionExtensions
{
    public static void AddPushNotifications(this IServiceCollection services, PushNotificationOptions options)
    {
        services.AddSingleton<PushNotificationResourceManager>();

        services.AddScoped<IPushNotificationTextProvider, PushNotificationTextProvider>();

        services.AddTransient<PnsConnectorFactory, PnsConnectorFactoryImpl>();

        services.AddTransient<IPushNotificationRegistrationService, PushService>();
        services.AddTransient<IPushNotificationSender, PushService>();

        if (options.Providers.Fcm is { Enabled: true })
        {
            services.AddFcm();
            services.AddSingleton<IOptions<FcmOptions>>(
                new OptionsWrapper<FcmOptions>(options.Providers.Fcm));
        }

        if (options.Providers.Apns is { Enabled: true })
        {
            services.AddApns();
            services.AddSingleton<IOptions<ApnsOptions>>(
                new OptionsWrapper<ApnsOptions>(options.Providers.Apns));
        }

        if (options.Providers.Dummy is { Enabled: true })
            services.AddDummy();

        if (options.Providers.Sse is { Enabled: true })
        {
            services.AddSse(options.Providers.Sse);

            if (options.Providers.Sse.EnableHealthCheck)
                services.AddHealthChecks().AddCheck<SseServerHealthCheck>("SseServer");
        }
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
        services.AddTransient<IJwtGenerator, JwtGenerator>();
        services.AddSingleton<ApnsJwtCache>();
    }

    private static void AddDummy(this IServiceCollection services)
    {
        services.AddTransient<DummyConnector>();
    }

    private static void AddSse(this IServiceCollection services, SseOptions options)
    {
        services.AddSingleton<ISseServerClient, SseServerClient>();

        services.AddHttpClient(nameof(SseServerClient), client => client.BaseAddress = new Uri(options.SseServerBaseAddress));

        services.AddScoped<ServerSentEventsConnector>();
    }
}

public class PushNotificationOptions
{
    [Required]
    public PushNotificationProviders Providers { get; set; } = null!;

    public class PushNotificationProviders
    {
        public required FcmOptions? Fcm { get; set; } = null!;

        public required ApnsOptions? Apns { get; set; } = null!;

        public required DummyOptions? Dummy { get; set; } = null!;

        public required SseOptions? Sse { get; set; } = null!;
    }
}

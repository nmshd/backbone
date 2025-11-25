using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Dummy;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddPushNotifications(PushNotificationConfiguration configuration)
        {
            services.AddSingleton<PushNotificationResourceManager>();

            services.AddSingleton<PushNotificationMetrics>();

            services.AddScoped<IPushNotificationTextProvider, PushNotificationTextProvider>();

            services.AddTransient<PnsConnectorFactory, PnsConnectorFactoryImpl>();

            services.AddTransient<IPushNotificationRegistrationService, PushService>();
            services.AddTransient<IPushNotificationSender, PushService>();

            if (configuration.Providers.Fcm is { Enabled: true })
                services.AddFcm(configuration.Providers.Fcm);

            if (configuration.Providers.Apns is { Enabled: true })
                services.AddApns(configuration.Providers.Apns);

            if (configuration.Providers.Dummy is { Enabled: true })
                services.AddDummy();

            if (configuration.Providers.Sse is { Enabled: true })
            {
                services.AddSse(configuration.Providers.Sse);
            }
        }

        private void AddFcm(FcmConfiguration configuration)
        {
            services.AddSingleton<FirebaseMessagingFactory>();
            services.AddTransient<FirebaseCloudMessagingConnector>();
            services.Configure<FcmConfiguration, FcmConfigurationValidator>(configuration);
        }

        private void AddApns(ApnsConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddTransient<ApplePushNotificationServiceConnector>();
            services.AddTransient<IJwtGenerator, JwtGenerator>();
            services.AddSingleton<ApnsJwtCache>();
            services.Configure<ApnsConfiguration, ApnsConfigurationValidator>(configuration);
        }

        private void AddDummy()
        {
            services.AddTransient<DummyConnector>();
        }

        private void AddSse(SseConfiguration configuration)
        {
            services.AddSingleton<ISseServerClient, SseServerClient>();

            services.AddHttpClient(nameof(SseServerClient), client => client.BaseAddress = new Uri(configuration.SseServerBaseAddress));

            services.AddScoped<ServerSentEventsConnector>();

            if (configuration.EnableHealthCheck)
                services.AddHealthChecks().AddCheck<SseServerHealthCheck>("SseServer");
        }
    }
}

public class PushNotificationConfiguration
{
    [Required]
    public required PushNotificationProviders Providers { get; init; }

    public class PushNotificationProviders
    {
        public required FcmConfiguration? Fcm { get; init; }

        public required ApnsConfiguration? Apns { get; init; }

        public required DummyConfiguration? Dummy { get; init; }

        public required SseConfiguration? Sse { get; init; }
    }
}

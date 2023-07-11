using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.AzureNotificationHub;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Dummy;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.FirebaseCloudMessaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public static class IServiceCollectionExtensions
{
    public const string PROVIDER_AZURE_NOTIFICATION_HUB = "AzureNotificationHub";
    public const string PROVIDER_DIRECT = "Direct";
    public const string PROVIDER_DUMMY = "Dummy";

    public static void AddPushNotifications(this IServiceCollection services, PushNotificationOptions options)
    {
        switch (options.Provider)
        {
            case PROVIDER_AZURE_NOTIFICATION_HUB:
                services.AddAzureNotificationHubPushNotifications(options.AzureNotificationHub);
                break;
            case PROVIDER_DUMMY: 
                services.AddDummyPushNotifications();
                break;
            case PROVIDER_DIRECT:

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(options.FCM_Service_Account),
                });

                services.AddTransient<PnsConnectorFactory, PnsConnectorFactoryImpl>();
                services.AddTransient<FirebaseCloudMessagingConnector>();
                services.AddTransient<IPushService, DirectPushService>();
                break;
            default:
                throw new Exception($"Push Notification Provider {options.Provider} does not exist.");
        }
    }
}

public class PushNotificationOptions
{
    [Required]
    [RegularExpression(
        $"{IServiceCollectionExtensions.PROVIDER_AZURE_NOTIFICATION_HUB}|{IServiceCollectionExtensions.PROVIDER_DIRECT}|{IServiceCollectionExtensions.PROVIDER_DUMMY}")]
    public string Provider { get; set; } = IServiceCollectionExtensions.PROVIDER_AZURE_NOTIFICATION_HUB;

    public AzureNotificationHub.IServiceCollectionExtensions.AzureNotificationHubPushNotificationsOptions AzureNotificationHub { get; set; }

    public string FCM_Service_Account { get; set; }
}

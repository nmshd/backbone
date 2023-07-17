using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.AzureNotificationHub;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Dummy;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public static class IServiceCollectionExtensions
{
    public const string PROVIDER_AZURE_NOTIFICATION_HUB = "AzureNotificationHub";
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
            default:
                throw new Exception($"Push Notification Provider {options.Provider} does not exist.");
        }
    }
}

public class PushNotificationOptions
{
    [Required]
    [RegularExpression(
        $"{IServiceCollectionExtensions.PROVIDER_AZURE_NOTIFICATION_HUB}|{IServiceCollectionExtensions.PROVIDER_DUMMY}")]
    public string Provider { get; set; } = IServiceCollectionExtensions.PROVIDER_AZURE_NOTIFICATION_HUB;

    public AzureNotificationHub.IServiceCollectionExtensions.AzureNotificationHubPushNotificationsOptions AzureNotificationHub { get; set; }
}

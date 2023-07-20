using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.AzureNotificationHub;

public class AzureNotificationHubPushService : IPushService
{
    private static readonly NotificationPlatform[] SUPPORTED_PLATFORMS = { NotificationPlatform.Fcm, NotificationPlatform.Apns };
    private readonly ILogger<AzureNotificationHubPushService> _logger;
    private readonly NotificationHubClient _notificationHubClient;

    public AzureNotificationHubPushService(NotificationHubClient notificationHubClient, ILogger<AzureNotificationHubPushService> logger)
    {
        _notificationHubClient = notificationHubClient;
        _logger = logger;
    }

    public async Task UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, CancellationToken cancellationToken)
    {
        var installation = new Installation
        {
            InstallationId = deviceId,
            PushChannel = handle.Value,
            Tags = GetNotificationTags(address),
            Platform = handle.Platform switch
            {
                PushNotificationPlatform.Apns => NotificationPlatform.Apns,
                PushNotificationPlatform.Fcm => NotificationPlatform.Fcm,
                _ => throw new ArgumentException($"There is no corresponding platform for '{handle.Platform}'.", nameof(handle.Platform))
            }
        };

        await _notificationHubClient.CreateOrUpdateInstallationAsync(installation, cancellationToken);

        _logger.LogTrace("New device successfully registered.");
    }

    public async Task SendNotification(IdentityAddress recipient, object pushNotification, CancellationToken cancellationToken)
    {
        var notificationContent = new NotificationContent(recipient, pushNotification);
        var notificationId = GetNotificationId(pushNotification);
        var (title, body) = GetNotificationText(pushNotification);

        foreach (var notificationPlatform in SUPPORTED_PLATFORMS)
        {
            var notification = NotificationBuilder
                .Create(notificationPlatform)
                .SetNotificationText(title, body)
                .SetTag(notificationId)
                .AddContent(notificationContent)
                .Build();

            await _notificationHubClient.SendNotificationAsync(notification, GetNotificationTags(recipient), cancellationToken);

            _logger.LogTrace($"Successfully sent push notification to identity '{recipient}' on platform '{notificationPlatform}': {notification.ToJson()}");
        }
    }

    private static (string Title, string Body) GetNotificationText(object pushNotification)
    {
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationTextAttribute>();
        return attribute == null ? ("", "") : (attribute.Title, attribute.Body);
    }

    private static int GetNotificationId(object pushNotification)
    {
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationIdAttribute>();
        return attribute?.Value ?? 0;
    }

    private static IList<string> GetNotificationTags(IdentityAddress identityId)
    {
        var identityTag = "identityId:" + identityId;
        return new[] { identityTag };
    }
}

public static class TypeExtensions
{
    public static T GetCustomAttribute<T>(this Type type) where T : Attribute
    {
        return (T)type.GetCustomAttribute(typeof(T));
    }
}

public static class NotificationExtensions
{
    private static readonly JsonSerializerOptions SERIALIZER_OPTIONS = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string ToJson(this Notification notification)
    {
        return JsonSerializer.Serialize(notification, SERIALIZER_OPTIONS);
    }
}

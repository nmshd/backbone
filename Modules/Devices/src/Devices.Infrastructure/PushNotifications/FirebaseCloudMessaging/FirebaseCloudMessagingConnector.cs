using System.Collections.Immutable;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.AzureNotificationHub;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.FirebaseCloudMessaging;
public class FirebaseCloudMessagingConnector : IPnsConnector
{
    private readonly FirebaseMessaging _firebaseMessaging;
    private readonly ILogger<FirebaseCloudMessagingConnector> _logger;

    public FirebaseCloudMessagingConnector(ILogger<FirebaseCloudMessagingConnector> logger, FirebaseMessaging firebaseMessaging)
    {
        _firebaseMessaging = firebaseMessaging;
        _logger = logger;
    }

    public async Task Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, object notification)
    {
        var recipients = registrations.Select(r => r.Handle.Value).ToList();
        if (recipients.Count > 500)
        {
            _logger.LogWarning($"There was an attempt to send an FCM MulticastMessage to more than 500 devices ({recipients.Count}) for the recipient with address '{recipient}'.");
        }

        var (notificationTitle, notificationBody) = GetNotificationText(notification);
        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(recipient, notification);

        var message = new FcmMessageBuilder()
            .AddContent(notificationContent)
            .SetNotificationText(notificationTitle, notificationBody)
            .SetTag(notificationId)
            .SetTokens(recipients.ToImmutableList())
            .Build();

        await _firebaseMessaging.SendMulticastAsync(message);
    }

    private static (string Title, string Body) GetNotificationText(object pushNotification)
    {
        switch (pushNotification)
        {
            case null:
                return ("", "");
            case JsonElement jsonElement:
            {
                var notification = jsonElement.Deserialize<NotificationTextAttribute>();
                return notification == null ? ("", "") : (notification.Title, notification.Body);
            }
            default:
            {
                var attribute = pushNotification.GetType().GetCustomAttribute<NotificationTextAttribute>();
                return attribute == null ? ("", "") : (attribute.Title, attribute.Body);
            }
        }
    }

    private static int GetNotificationId(object pushNotification)
    {
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationIdAttribute>();
        return attribute?.Value ?? 0;
    }
}
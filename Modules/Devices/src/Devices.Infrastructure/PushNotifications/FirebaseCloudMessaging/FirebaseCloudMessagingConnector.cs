using System.Collections.Immutable;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.AzureNotificationHub;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using FirebaseAdmin.Messaging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.FirebaseCloudMessaging;
public class FirebaseCloudMessagingConnector : IPnsConnector
{
    public async Task Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient,
        object notification)
    {
        var recipients = registrations.Select(r => r.Handle.Value).ToList();
        var recipientsBatches = recipients.Split(500);

        var (notificationTitle, notificationBody) = GetNotificationText(notification);
        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(recipient, notification);

        foreach (var batch in recipientsBatches)
        {
            var message = new FcmMessageBuilder()
                .AddContent(notificationContent)
                .SetNotificationText(notificationTitle, notificationBody)
                .SetTag(notificationId)
                .SetTokens(batch.ToImmutableList())
                .Build();

            await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
        }
    }

    private static MulticastMessage CreateMulticastMessage(IEnumerable<string> tokens, string notificationTitle, string notificationBody)
    {
        var data = new Dictionary<string, string>
        {
            {"android_channel_id", "ENMESHED"},
            {"content-available", "1"},
            {"tag", "1"}
        };

        var message = new MulticastMessage
        {
            Tokens = tokens.ToList(),
            Notification = new()
            {
                Title = notificationTitle,
                Body = notificationBody
            },
            Data = data.AsReadOnly()
        };
        return message;
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
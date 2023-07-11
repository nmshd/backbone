using System.Text.Json;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.AzureNotificationHub;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using FirebaseAdmin.Messaging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.FirebaseCloudMessaging;
public class FirebaseCloudMessagingConnector : IPnsConnector
{
    public FirebaseCloudMessagingConnector()
    { }

    public async Task Send(IEnumerable<PnsRegistration> registrations, object notification)
    {
        var recipients = registrations.Select(r => r.Handle.Value).ToList();
        var recipientsLists = recipients.Split(500);
        var data = new Dictionary<string, string>();
        var (notificationTitle, notificationBody) = GetNotificationText(notification);

        foreach (var list in recipientsLists)
        {
            var message = CreateMulticastMessage(list, notificationTitle, notificationBody, data);
            await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
        }
    }

    private static MulticastMessage CreateMulticastMessage(IEnumerable<string> tokens, string notificationTitle,
        string notificationBody, Dictionary<string, string> data)
    {
        var message = new MulticastMessage()
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
}
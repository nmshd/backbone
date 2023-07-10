using System.Text.Json;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
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
            var message = new MulticastMessage()
            {
                Tokens = list.ToList(),
                Notification = new()
                {
                    Title = notificationTitle,
                    Body = notificationBody
                },
                Data = data.AsReadOnly()
            };

            await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
        }
    }

    private static (string Title, string Body) GetNotificationText(object pushNotification)
    {
        if (pushNotification == null)
            return ("", "");
        var notification = JsonSerializer.Deserialize<NotificationTextAttribute>((JsonElement)pushNotification);
        return notification == null ? ("", "") : (notification.Title, notification.Body);
    }
}
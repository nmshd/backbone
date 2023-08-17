using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;

public class FirebaseCloudMessagingConnector : IPnsConnector
{
    private readonly FirebaseMessagingFactory _firebaseMessagingFactory;

    public FirebaseCloudMessagingConnector(FirebaseMessagingFactory firebaseMessagingFactory)
    {
        _firebaseMessagingFactory = firebaseMessagingFactory;
    }

    public async Task Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, object notification)
    {
        var uniqueAppIds = registrations.DistinctBy(r => r.AppId).Select(r => r.AppId);
        foreach (var appId in uniqueAppIds)
        {
            var recipients = registrations.Where(r => r.AppId == appId).Select(r => r.Handle.Value).ToList();

            var (notificationTitle, notificationBody) = GetNotificationText(notification);
            var notificationId = GetNotificationId(notification);
            var notificationContent = new NotificationContent(recipient, notification);

            var message = new FcmMessageBuilder()
                .AddContent(notificationContent)
                .SetNotificationText(notificationTitle, notificationBody)
                .SetTag(notificationId)
                .SetTokens(recipients.ToImmutableList())
                .Build();

            var firebaseMessaging = _firebaseMessagingFactory.CreateForAppId(appId);
            await firebaseMessaging.SendMulticastAsync(message);
        }
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

public static class TypeExtensions
{
    public static T GetCustomAttribute<T>(this Type type) where T : Attribute
    {
        return (T)type.GetCustomAttribute(typeof(T));
    }
}

using System.Reflection;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using FirebaseAdmin.Messaging;
using Newtonsoft.Json;

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

        foreach ( var list in recipientsLists ) {
            var message = new MulticastMessage() { 
                Tokens = list.ToList(),
                Notification = new() { 
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
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationTextAttribute>();
        return attribute == null ? ("", "") : (attribute.Title, attribute.Body);
    }
}

public static class TypeExtensions
{
    public static T GetCustomAttribute<T>(this Type type) where T : Attribute
    {
        return (T)type.GetCustomAttribute(typeof(T));
    }
}

public sealed class FCMMessage
{
    [JsonProperty("data")]
    public FCMData Data { get; set; }

    [JsonProperty("notification")]
    public object Notification { get; set; }

    [JsonProperty("registration_ids")]
    public IEnumerable<string> Recipients { get; set; }
}

public sealed class FCMData
{
    [JsonProperty("android_channel_id")]
    public string AndroidChannelId;

    [JsonProperty("content_available")]
    public string ContentAvailable;

    [JsonProperty("content")]
    public Dictionary<string, dynamic> Content;
}

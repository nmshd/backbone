using System.Text.Json.Serialization;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public class NotificationContent
{
    private const string PUSH_NOTIFICATION_POSTFIX = "PushNotification";

    public NotificationContent(IdentityAddress recipient, DevicePushIdentifier devicePushIdentifier, object pushNotification)
    {
        EventName = DetermineEventName(pushNotification);
        AccountReference = recipient;
        DevicePushIdentifier = devicePushIdentifier;
        Payload = pushNotification;
        SentAt = SystemTime.UtcNow;
    }

    private string DetermineEventName(object pushNotification)
    {
        var notificationTypeName = pushNotification.GetType().Name;

        if (notificationTypeName.Contains(PUSH_NOTIFICATION_POSTFIX))
            return notificationTypeName.Replace(PUSH_NOTIFICATION_POSTFIX, "");

        return "dynamic";
    }

    [JsonPropertyName("accRef")]
    public string AccountReference { get; }
    [JsonPropertyName("devicePushIdentifier")]
    public string DevicePushIdentifier { get; }

    [JsonPropertyName("eventName")]
    public string EventName { get; set; }

    [JsonPropertyName("sentAt")]
    public DateTime SentAt { get; }

    [JsonPropertyName("payload")]
    public object Payload { get; set; }
}

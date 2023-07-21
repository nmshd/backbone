using System.Text.Json.Serialization;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public class NotificationContent
{
    private const string PUSH_NOTIFICATION_POSTFIX = "PushNotification";

    public NotificationContent(IdentityAddress recipient, object pushNotification)
    {
        EventName = DetermineEventName(pushNotification);
        AccountReference = recipient;
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

    [JsonPropertyName("eventName")]
    public string EventName { get; set; }

    [JsonPropertyName("sentAt")]
    public DateTime SentAt { get; }

    [JsonPropertyName("payload")]
    public object Payload { get; set; }
}

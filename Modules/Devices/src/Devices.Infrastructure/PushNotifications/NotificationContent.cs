using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public class NotificationContent
{
    public NotificationContent(IdentityAddress recipient, DevicePushIdentifier devicePushIdentifier, IPushNotification pushNotification)
    {
        EventName = pushNotification.GetEventName();
        AccountReference = recipient;
        DevicePushIdentifier = devicePushIdentifier;
        Payload = pushNotification;
        SentAt = SystemTime.UtcNow;
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

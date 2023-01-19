using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.NotificationHubs;

namespace Devices.Infrastructure.PushNotifications;

/// <summary>
///     See corresponding Unit Tests for an example of a built notification.
/// </summary>
public class ApnsNotificationBuilder : NotificationBuilder
{
    private readonly Payload _notification = new();

    public ApnsNotificationBuilder()
    {
        AddHeader("apns-priority", "5");
    }
    
    public override NotificationBuilder AddContent(NotificationContent content)
    {
        _notification.Content = content;

        SetContentAvailable(true);

        return this;
    }

    public override NotificationBuilder SetNotificationText(string title, string body)
    {
        if (!string.IsNullOrWhiteSpace(title))
            _notification.APS.Alert.Title = title;

        if (!string.IsNullOrWhiteSpace(body))
            _notification.APS.Alert.Body = body;

        return this;
    }
    
    private void SetContentAvailable(bool contentAvailable)
    {
        _notification.APS.ContentAvailable = contentAvailable ? "1" : "0";
    }

    public override NotificationBuilder SetTag(int notificationId)
    {
        _notification.NotificationId = notificationId;
        return this;
    }

    public override Notification Build()
    {
        var serializedPayload = JsonSerializer.Serialize(_notification, _jsonSerializerOptions);
        var notification = new AppleNotification(serializedPayload, _headers);
        return notification;
    }

    private class Payload
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local

        [JsonPropertyName("notId")]
        public int NotificationId { get; set; }

        [JsonPropertyName("content")]
        public NotificationContent Content { get; set; }

        [JsonPropertyName("aps")]
        public PayloadAps APS { get; } = new();
        
        public class PayloadAps
        {
            [JsonPropertyName("content-available")]
            public string ContentAvailable { get; set; }

            [JsonPropertyName("alert")]
            public ApsAlert Alert { get; } = new();

            public class ApsAlert
            {
                [JsonPropertyName("title")]
                public string Title { get; set; }

                [JsonPropertyName("body")]
                public string Body { get; set; }
            }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.NotificationHubs;

namespace Devices.Infrastructure.PushNotifications;

/// <summary>
///     See corresponding Unit Tests for an example of a built notification.
/// </summary>
public class FcmNotificationBuilder : NotificationBuilder
{
    private readonly Payload _notification = new();

    public FcmNotificationBuilder()
    {
        SetAndroidChannelId("ENMESHED");
    }
    
    private void SetAndroidChannelId(string channelId)
    {
        _notification.Data.AndroidChannelId = channelId;
    }

    public override NotificationBuilder AddContent(NotificationContent content)
    {
        _notification.Data.Content = content;

        SetContentAvailable(true);

        return this;
    }

    public override NotificationBuilder SetNotificationText(string title, string body)
    {
        if (!string.IsNullOrWhiteSpace(title))
            _notification.Notification.Title = title;

        if (!string.IsNullOrWhiteSpace(body))
            _notification.Notification.Body = body;

        return this;
    }
    
    private void SetContentAvailable(bool contentAvailable)
    {
        _notification.Data.ContentAvailable = contentAvailable ? "1" : "0";
    }

    public override NotificationBuilder SetTag(int notificationId)
    {
        _notification.Notification.Tag = notificationId.ToString();
        return this;
    }

    public override Notification Build()
    {
        var serializedPayload = JsonSerializer.Serialize(_notification, _jsonSerializerOptions);
        var notification = new FcmNotification(serializedPayload);
        return notification;
    }

    private class Payload
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local

        [JsonPropertyName("data")]
        public PayloadData Data { get; } = new();

        public PayloadNotification Notification { get; } = new();

        public class PayloadNotification
        {
            [JsonPropertyName("tag")]
            public string Tag { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("body")]
            public string Body { get; set; }
        }

        public class PayloadData
        {
            [JsonPropertyName("android_channel_id")]
            public string AndroidChannelId { get; set; }

            [JsonPropertyName("content-available")]
            public string ContentAvailable { get; set; } // TODO: make boolean?

            [JsonPropertyName("content")]
            public NotificationContent Content { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}

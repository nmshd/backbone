using System.Text.Json;
using FirebaseAdmin.Messaging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;

public class FcmMessageBuilder
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new DateTimeConverter() }, PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly Message _message;

    private readonly Dictionary<string, string> _data;

    public FcmMessageBuilder()
    {
        _message = new Message
        {
            Notification = new Notification(),
            Android = new AndroidConfig
            {
                Notification = new AndroidNotification()
            }
        };

        _data = new Dictionary<string, string>();

        SetAndroidChannelId("ENMESHED");
    }

    private void SetAndroidChannelId(string channelId)
    {
        _data["android_channel_id"] = channelId;
        _message.Android.Notification.ChannelId = channelId;
    }

    public FcmMessageBuilder AddContent(NotificationContent? content)
    {
        if (content == null)
            return this;

        _data["content"] = JsonSerializer.Serialize(content, _jsonSerializerOptions);
        SetContentAvailable(true);

        return this;
    }

    public FcmMessageBuilder SetNotificationText(string title, string body)
    {
        if (!string.IsNullOrWhiteSpace(title))
            _message.Notification.Title = title;

        if (!string.IsNullOrWhiteSpace(body))
            _message.Notification.Body = body;

        return this;
    }

    private void SetContentAvailable(bool contentAvailable)
    {
        _data["content-available"] = contentAvailable ? "1" : "0";
    }

    public FcmMessageBuilder SetTag(string? notificationId)
    {
        if (notificationId == null)
            return this;

        _message.Android.Notification.Tag = notificationId;
        _message.Android.CollapseKey = notificationId;

        return this;
    }

    public FcmMessageBuilder SetToken(string token)
    {
        _message.Token = token;
        return this;
    }

    public Message Build()
    {
        _message.Data = _data;
        return _message;
    }
}

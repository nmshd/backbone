﻿using System.Text.Json;
using FirebaseAdmin.Messaging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;

/// <summary>
///     See corresponding Unit Tests for an example of a built notification.
/// </summary>
public class FcmMessageBuilder
{
    protected JsonSerializerOptions _jsonSerializerOptions = new() { Converters = { new DateTimeConverter() }, PropertyNamingPolicy = JsonNamingPolicy.CamelCase /*, Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement)*/};
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

    public FcmMessageBuilder AddContent(NotificationContent content)
    {
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

    public FcmMessageBuilder SetTag(int notificationId)
    {
        _data["tag"] = notificationId.ToString();
        _message.Android.CollapseKey = notificationId.ToString();
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

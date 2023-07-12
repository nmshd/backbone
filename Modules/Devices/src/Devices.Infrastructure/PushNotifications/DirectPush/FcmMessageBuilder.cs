﻿using System.Text.Json;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.AzureNotificationHub;
using FirebaseAdmin.Messaging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

/// <summary>
///     See corresponding Unit Tests for an example of a built notification.
/// </summary>
public class FcmMessageBuilder
{
    private readonly MulticastMessage _message = new()
    {
        Notification = new Notification(),
        Android = new()
        {
            Notification = new()
        }
    };

    private readonly Dictionary<string, string> _data = new();

    public FcmMessageBuilder()
    {
        SetAndroidChannelId("ENMESHED");
    }

    private void SetAndroidChannelId(string channelId)
    {
        _data["android_channel_id"] = channelId;
        _message.Android.Notification.ChannelId = channelId;
    }

    public FcmMessageBuilder AddContent(NotificationContent content)
    {
        _data["content"] = JsonSerializer.Serialize(content);
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

    public FcmMessageBuilder SetTokens(IEnumerable<string> tokens)
    {
        var tokenList = tokens.ToList();

        if (tokenList.Count > 500)
        {
            throw new ArgumentOutOfRangeException(nameof(tokens), "FCM Messages cannot have more than 500 tokens.");
        }

        _message.Tokens = tokenList.ToList();
        return this;
    }

    public MulticastMessage Build()
    {
        _message.Data = _data.AsReadOnly();
        return _message;
    }
}

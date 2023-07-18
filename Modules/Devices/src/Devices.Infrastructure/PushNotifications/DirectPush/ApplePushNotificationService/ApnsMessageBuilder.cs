﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Enmeshed.Tooling.Extensions;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class ApnsMessageBuilder
{
    private readonly Payload _notification = new();
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { Converters = { new DateTimeConverter() }, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly HttpRequestMessage _request;

    public ApnsMessageBuilder(string appBundleIdentifier, string path, string jwt)
    {
        _request = new HttpRequestMessage(HttpMethod.Post, new Uri(path))
        {
            Version = new Version(2, 0)
        };
        _request.Headers.Add("apns-topic", appBundleIdentifier);
        _request.Headers.Add("apns-expiration", "0");
        _request.Headers.Add("apns-priority", "5");
        _request.Headers.Add("apns-push-type", "alert");
        _request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", jwt);
    }

    public ApnsMessageBuilder AddContent(NotificationContent content)
    {
        _notification.Content = content;
        SetContentAvailable(true);
        return this;
    }

    public ApnsMessageBuilder SetNotificationText(string title, string body)
    {
        if (!string.IsNullOrWhiteSpace(title))
            _notification.Aps.Alert.Title = title;

        if (!string.IsNullOrWhiteSpace(body))
            _notification.Aps.Alert.Body = body;

        return this;
    }

    private void SetContentAvailable(bool contentAvailable)
    {
        _notification.Aps.ContentAvailable = contentAvailable ? "1" : "0";
    }

    public ApnsMessageBuilder SetTag(int notificationId)
    {
        _notification.NotificationId = notificationId;
        return this;
    }

    public HttpRequestMessage Build()
    {
        var payload = JsonSerializer.Serialize(_notification, _jsonSerializerOptions);
        _request.Content = new StringContent(payload);
        return _request;
    }

    private class Payload
    {
        [JsonPropertyName("notId")]
        public int NotificationId { get; set; }

        [JsonPropertyName("content")]
        public NotificationContent Content { get; set; }

        [JsonPropertyName("aps")]
        public PayloadAps Aps { get; } = new();

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
    }

    private class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalString());
        }
    }

    public object SetTopic()
    {
        throw new NotImplementedException();
    }
}

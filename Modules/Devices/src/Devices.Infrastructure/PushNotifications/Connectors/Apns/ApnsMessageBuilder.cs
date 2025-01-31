using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.Tooling.Extensions;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;

public class ApnsMessageBuilder
{
    private readonly Payload _notification = new();
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { Converters = { new DateTimeConverter() }, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly HttpRequestMessage _request;

    public ApnsMessageBuilder(string appBundleIdentifier, string path, string jwt)
    {
        _request = new HttpRequestMessage(HttpMethod.Post, new Uri(path))
        {
            Version = HttpVersion.Version30,
            Headers =
            {
                { "apns-topic", appBundleIdentifier },
                { "apns-expiration", "0" },
                { "apns-priority", "5" },
                { "apns-push-type", "alert" }
            }
        };

        _request.Headers.Authorization = new AuthenticationHeaderValue("bearer", jwt);
    }

    public ApnsMessageBuilder AddContent(NotificationContent content)
    {
        _notification.Content = content;
        SetContentAvailable(true);
        return this;
    }

    private void SetContentAvailable(bool contentAvailable)
    {
        _notification.Aps.ContentAvailable = contentAvailable ? "1" : "0";
    }

    public ApnsMessageBuilder SetNotificationText(string title, string body)
    {
        if (!string.IsNullOrWhiteSpace(title))
            _notification.Aps.Alert.Title = title;

        if (!string.IsNullOrWhiteSpace(body))
            _notification.Aps.Alert.Body = body;

        return this;
    }

    public ApnsMessageBuilder SetNotificationId(string? notificationId)
    {
        if (notificationId == null)
            return this;

        _notification.NotificationId = notificationId;
        _request.Headers.Add("apns-collapse-id", notificationId);

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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? NotificationId { get; set; }

        [JsonPropertyName("content")]
        public NotificationContent? Content { get; set; }

        [JsonPropertyName("aps")]
        public PayloadAps Aps { get; } = new();

        public class PayloadAps
        {
            [JsonPropertyName("content-available")]
            public string? ContentAvailable { get; set; }

            [JsonPropertyName("alert")]
            public ApsAlert Alert { get; } = new();

            public class ApsAlert
            {
                [JsonPropertyName("title")]
                public string? Title { get; set; }

                [JsonPropertyName("body")]
                public string? Body { get; set; }
            }
        }
    }

    private class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalString());
        }
    }
}

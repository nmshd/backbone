using System.Text.Json;
using System.Text.Json.Serialization;
using Enmeshed.Tooling.Extensions;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class ApnsMessageBuilder
{
    private readonly Payload _notification = new();
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { Converters = { new DateTimeConverter() }, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly RestRequest _request;

    public ApnsMessageBuilder(string server, string appBundleIdentifier, string device, string jwt)
    {
        _request = new RestRequest(new PathString(server).Add(device), Method.Post);
        _request.AddHeader("apns-topic", appBundleIdentifier);
        _request.AddHeader("Authorization", $"Bearer {jwt}");
        _request.AddHeader("apns-expiration", 0);
        _request.AddHeader("apns-priority", 5);
        _request.AddHeader("apns-push-type", "alert");
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

    public RestRequest Build()
    {
        var payload = JsonSerializer.Serialize(_notification, _jsonSerializerOptions);
        _request.AddBody(payload);
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

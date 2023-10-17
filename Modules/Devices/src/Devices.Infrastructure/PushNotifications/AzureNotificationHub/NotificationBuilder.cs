using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.Tooling.Extensions;
using Microsoft.Azure.NotificationHubs;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.AzureNotificationHub;

public abstract class NotificationBuilder
{
    protected JsonSerializerOptions _jsonSerializerOptions = new() { Converters = { new DateTimeConverter() }, PropertyNamingPolicy = JsonNamingPolicy.CamelCase /*, Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement)*/};
    protected readonly Dictionary<string, string> _headers = new();

    public static NotificationBuilder Create(NotificationPlatform platform)
    {
        NotificationBuilder builder = platform switch
        {
            NotificationPlatform.Fcm => new FcmNotificationBuilder(),
            NotificationPlatform.Apns => new ApnsNotificationBuilder(),
            _ => throw new ArgumentException($"The platform {platform} is not supported.")
        };

        return builder;
    }

    public abstract NotificationBuilder AddContent(NotificationContent content);

    public abstract NotificationBuilder SetNotificationText(string title, string body);

    public abstract NotificationBuilder SetTag(int notificationId);

    public NotificationBuilder AddHeader(string name, string value)
    {
        _headers.Add(name, value);
        return this;
    }

    public abstract Notification Build();
}

public class DateTimeConverter : JsonConverter<DateTime>
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

using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

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

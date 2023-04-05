using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backbone.API.Mvc.JsonConverters.Common;

public class UtcDateTimeConverter : JsonConverter<DateTime>
{
    internal const string FORMAT = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffZ";

    public UtcDateTimeConverter()
    {
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime);
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue == null)
            throw new Exception("Value cannot be null");

        return DateTime.Parse(stringValue);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString(FORMAT));
    }
}

public class NullableUtcDateTimeConverter : JsonConverter<DateTime?>
{
    public NullableUtcDateTimeConverter()
    {
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime?);
    }

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        return stringValue == null ? null : DateTime.Parse(stringValue);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value.ToUniversalTime().ToString(UtcDateTimeConverter.FORMAT));
    }
}
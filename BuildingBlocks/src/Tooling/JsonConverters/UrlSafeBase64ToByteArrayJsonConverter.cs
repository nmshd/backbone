using System.Text.Json;
using System.Text.Json.Serialization;
using NeoSmart.Utils;

namespace Backbone.Tooling.JsonConverters;

public class UrlSafeBase64ToByteArrayJsonConverter : JsonConverter<byte[]>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(byte[]);
    }

    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        try
        {
            return UrlBase64.Decode(stringValue);
        }
        catch (FormatException e)
        {
            throw new JsonException(e.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        var stringValue = UrlBase64.Encode(value);
        writer.WriteStringValue(stringValue);
    }
}

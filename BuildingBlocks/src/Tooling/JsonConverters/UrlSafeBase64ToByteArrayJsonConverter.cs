using System.Buffers.Text;
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
            return Base64Url.DecodeFromChars(stringValue);
        }
        catch (FormatException e)
        {
            throw new JsonException(e.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        var stringValue = Base64Url.EncodeToChars(value);
        writer.WriteStringValue(stringValue);
    }
}

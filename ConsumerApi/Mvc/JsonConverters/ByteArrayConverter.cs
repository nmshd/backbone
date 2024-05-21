using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Mvc.JsonConverters;

public class JsonToByteArrayConverter : JsonConverter<byte[]?>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!reader.TryGetBytesFromBase64(out var result))
            throw new JsonException("Byte array deserialization failed.");

        return result;
    }

    public override void Write(Utf8JsonWriter writer, byte[]? value, JsonSerializerOptions options)
    {
        writer.WriteBase64StringValue(value);
    }
}

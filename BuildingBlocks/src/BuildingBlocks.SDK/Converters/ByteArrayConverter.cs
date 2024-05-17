using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backbone.BuildingBlocks.SDK.Converters;

public class JsonToByteArrayConverter : JsonConverter<byte[]?>
{
    // Converts base64 encoded string to byte[].
    public override byte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!reader.TryGetBytesFromBase64(out var result))
        {
            throw new Exception("Add your fancy exception message here...");
        }

        return result;
    }

    // Converts byte[] to base64 encoded string.
    public override void Write(Utf8JsonWriter writer, byte[]? value, JsonSerializerOptions options)
    {
        writer.WriteBase64StringValue(value);
    }
}

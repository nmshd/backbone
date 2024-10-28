using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backbone.Modules.Tags.Domain;

public class TagInfo
{
    [Required]
    public Dictionary<string, string> DisplayNames { get; set; } = [];

    [JsonConverter(typeof(PascalCaseDictionaryConverter<TagInfo>))]
    public Dictionary<string, TagInfo> Children { get; set; } = [];
}

public class PascalCaseDictionaryConverter<TValue> : JsonConverter<Dictionary<string, TValue>>
{
    public override Dictionary<string, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Deserialization is not supported by this converter.");
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var key in value.Keys)
        {
            var value2 = value[key];
            writer.WritePropertyName(key);
            JsonSerializer.Serialize(writer, value2, options);
        }

        writer.WriteEndObject();
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backbone.Modules.Tags.Application.Tags.Queries.ListTags;

[JsonConverter(typeof(ListTagsResponseConverter))]
public class ListTagsResponse : Dictionary<string, IEnumerable<TagInfo>>
{
    public ListTagsResponse(IReadOnlyDictionary<string, IEnumerable<TagInfo>> tags) : base(tags)
    {
    }
}

public class ListTagsResponseConverter : JsonConverter<ListTagsResponse>
{
    public override ListTagsResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return null;
    }

    public override void Write(Utf8JsonWriter writer, ListTagsResponse value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var (name, tags) in value)
        {
            writer.WriteStartArray(name);
            foreach (var tag in tags)
            {
                writer.WriteStartObject();
                writer.WriteString("tag", tag.Tag);
                if (tag.Title != null) writer.WriteString("title", tag.Title);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }
}

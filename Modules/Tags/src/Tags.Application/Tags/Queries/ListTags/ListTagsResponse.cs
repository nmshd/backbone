using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backbone.Modules.Tags.Application.Tags.Queries.ListTags;

[JsonConverter(typeof(ListTagsResponseConverter))]
public class ListTagsResponse : Dictionary<string, IEnumerable<TagInfo>>
{
    public IEnumerable<string> SupportedLanguages =>
        Values
            .SelectMany(tags => tags)
            .SelectMany(tag => tag.DisplayNames.Keys)
            .Distinct();

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

        writer.WriteStartArray("supportedLanguages");
        foreach (var lang in value.SupportedLanguages) writer.WriteStringValue(lang);
        writer.WriteEndArray();

        writer.WriteStartObject("tagsForAttributeValueTypes");
        foreach (var (name, tags) in value)
        {
            writer.WriteStartObject(name);
            foreach (var tag in tags) WriteTag(writer, tag);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();

        writer.WriteEndObject();
    }

    private void WriteTag(Utf8JsonWriter writer, TagInfo tag)
    {
        writer.WriteStartObject(tag.Tag);

        writer.WriteStartObject("displayNames");
        foreach (var (lang, title) in tag.DisplayNames) writer.WriteString(lang, title);
        writer.WriteEndObject();

        if (tag.Children.Count > 0)
        {
            writer.WriteStartObject("children");
            foreach (var child in tag.Children) WriteTag(writer, child);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }
}

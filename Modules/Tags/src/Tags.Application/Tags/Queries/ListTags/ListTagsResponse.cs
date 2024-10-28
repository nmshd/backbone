using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.Modules.Tags.Domain;

namespace Backbone.Modules.Tags.Application.Tags.Queries.ListTags;

[JsonConverter(typeof(ListTagsResponseJsonConverter))]
public class ListTagsResponse
{
    public required IEnumerable<string> SupportedLanguages { get; set; }
    public required Dictionary<string, Dictionary<string, TagInfo>> TagsForAttributeValueTypes { get; set; }
}

public class ListTagsResponseJsonConverter : JsonConverter<ListTagsResponse>
{
    public override ListTagsResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => null;

    public override void Write(Utf8JsonWriter writer, ListTagsResponse value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        WriteSupportedLanguages(writer, value.SupportedLanguages);
        WriteAttributes(writer, value.TagsForAttributeValueTypes);

        writer.WriteEndObject();
    }

    private void WriteSupportedLanguages(Utf8JsonWriter writer, IEnumerable<string> supportedLanguages)
    {
        writer.WriteStartArray("supportedLanguages");
        foreach (var lang in supportedLanguages) writer.WriteStringValue(lang);
        writer.WriteEndArray();
    }

    private void WriteAttributes(Utf8JsonWriter writer, Dictionary<string, Dictionary<string, TagInfo>> attributes)
    {
        writer.WriteStartObject("tagsForAttributeValueTypes");
        foreach (var (attributeName, attribute) in attributes)
        {
            writer.WriteStartObject(attributeName);
            foreach (var (tagName, tag) in attribute) WriteTag(writer, tagName, tag);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }

    private void WriteTag(Utf8JsonWriter writer, string tagName, TagInfo tag)
    {
        writer.WriteStartObject(tagName);

        writer.WriteStartObject("displayNames");
        foreach (var (lang, displayName) in tag.DisplayNames) writer.WriteString(lang, displayName);
        writer.WriteEndObject();

        if (tag.Children.Count > 0)
        {
            writer.WriteStartObject("children");
            foreach (var (childName, child) in tag.Children) WriteTag(writer, childName, child);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }
}

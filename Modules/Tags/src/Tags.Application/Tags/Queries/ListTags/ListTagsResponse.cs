using System.Text.Json.Serialization;
using Backbone.Modules.Tags.Domain;

namespace Backbone.Modules.Tags.Application.Tags.Queries.ListTags;

public class ListTagsResponse
{
    public required IEnumerable<string> SupportedLanguages { get; set; }

    [JsonConverter(typeof(PascalCaseDictionaryConverter<Dictionary<string, TagInfo>>))]
    public required Dictionary<string, Dictionary<string, TagInfo>> TagsForAttributeValueTypes { get; set; }
}

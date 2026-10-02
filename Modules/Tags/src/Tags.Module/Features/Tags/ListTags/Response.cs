using System.Text.Json.Serialization;
using Backbone.Modules.Tags.Domain;

namespace Backbone.Modules.Tags.Module.Features.Tags.ListTags;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListTagsResponse")]
public class Response
{
    public required IEnumerable<string> SupportedLanguages { get; set; }

    [JsonConverter(typeof(PascalCaseDictionaryConverter<Dictionary<string, TagInfo>>))]
    public required Dictionary<string, Dictionary<string, TagInfo>> TagsForAttributeValueTypes { get; set; }
}

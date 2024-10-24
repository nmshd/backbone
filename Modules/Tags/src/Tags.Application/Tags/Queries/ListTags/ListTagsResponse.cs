using Backbone.Modules.Tags.Domain;

namespace Backbone.Modules.Tags.Application.Tags.Queries.ListTags;

public class ListTagsResponse
{
    public required IEnumerable<string> SupportedLanguages { get; set; }
    public required Dictionary<string, Dictionary<string, TagInfo>> TagsForAttributeValueTypes { get; set; }
}

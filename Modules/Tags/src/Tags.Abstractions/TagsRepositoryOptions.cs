using Backbone.Modules.Tags.Domain;

namespace Backbone.Modules.Tags.Abstractions;

public class TagsRepositoryOptions
{
    public required List<string> SupportedLanguages { get; init; }
    public required Dictionary<string, Dictionary<string, TagInfo>> TagsForAttributeValueTypes { get; init; }
}

namespace Backbone.Modules.Tags.Application;

public class TagProvider
{
    public IReadOnlyDictionary<string, IEnumerable<TagInfo>> LegalTags => _tags;

    private readonly Dictionary<string, IEnumerable<TagInfo>> _tags = [];

    public TagProvider(IEnumerable<AttributeInfo> attributes)
    {
        foreach (var attribute in attributes)
        {
            _tags[attribute.Name] = attribute.Tags;
        }
    }
}

namespace Backbone.Modules.Tags.Application.Tags.Queries.ListTags;

public class ListTagsResponse : Dictionary<string, IEnumerable<TagInfo>>
{
    public ListTagsResponse(IReadOnlyDictionary<string, IEnumerable<TagInfo>> tags) : base(tags)
    {
    }
}

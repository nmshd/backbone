namespace Backbone.ConsumerApi.Sdk.Endpoints.Tags.Types;

public class TagDefinition : Dictionary<string, Tag>;

public class Tag
{
    public required Dictionary<string, string> DisplayNames { get; set; }
    public TagDefinition Children { get; set; } = [];
}

namespace Backbone.Modules.Tags.Application;

public class ApplicationOptions
{
    public required List<AttributeInfo> Attributes { get; set; }
}

public class AttributeInfo
{
    public required string Name { get; set; }
    public required List<TagInfo> Tags { get; set; }
}

public class TagInfo
{
    public required string Tag { get; set; }
    public required string Title { get; set; }
}

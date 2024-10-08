﻿namespace Backbone.Modules.Tags.Application;

public class TagProvider
{
    public IReadOnlyDictionary<string, IEnumerable<TagInfo>> LegalTags => _tags;

    private readonly Dictionary<string, IEnumerable<TagInfo>> _tags = [];

    public TagProvider(ApplicationOptions options)
    {
        foreach (var attribute in options.Attributes)
        {
            _tags[attribute.Name] = attribute.Tags;
        }
    }
}

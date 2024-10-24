using Backbone.Modules.Tags.Application;
using Backbone.Modules.Tags.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tags.Domain;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Tags.Infrastructure.Persistence.Repository;

public class TagsRepository : ITagsRepository
{
    private readonly List<string> _supportedLanguages;
    private readonly Dictionary<string, Dictionary<string, TagInfo>> _attributes;

    public TagsRepository(IOptions<ApplicationOptions> options)
    {
        _supportedLanguages = options.Value.SupportedLanguages;
        _attributes = options.Value.TagsForAttributeValueTypes;
    }

    public IEnumerable<string> GetSupportedLanguages() => _supportedLanguages;
    public Dictionary<string, Dictionary<string, TagInfo>> GetAttributes() => _attributes;
}

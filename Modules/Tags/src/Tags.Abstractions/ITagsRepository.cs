using Backbone.Modules.Tags.Domain;

namespace Backbone.Modules.Tags.Abstractions;

public interface ITagsRepository
{
    IEnumerable<string> ListSupportedLanguages();
    Dictionary<string, Dictionary<string, TagInfo>> ListAttributes();
}

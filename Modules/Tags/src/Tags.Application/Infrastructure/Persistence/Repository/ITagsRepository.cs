using Backbone.Modules.Tags.Domain;

namespace Backbone.Modules.Tags.Application.Infrastructure.Persistence.Repository;

public interface ITagsRepository
{
    IEnumerable<string> ListSupportedLanguages();
    Dictionary<string, Dictionary<string, TagInfo>> ListAttributes();
}

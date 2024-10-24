using Backbone.Modules.Tags.Domain;

namespace Backbone.Modules.Tags.Application.Infrastructure.Persistence.Repository;

public interface ITagsRepository
{
    public IEnumerable<string> GetSupportedLanguages();
    public Dictionary<string, Dictionary<string, TagInfo>> GetAttributes();
}

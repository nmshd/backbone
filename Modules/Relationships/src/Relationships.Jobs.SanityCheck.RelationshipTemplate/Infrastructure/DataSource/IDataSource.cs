using Backbone.Modules.Relationships.Domain.Ids;

namespace Relationships.Jobs.SanityCheck.RelationshipTemplate.Infrastructure.DataSource
{
    public interface IDataSource
    {
        Task<IEnumerable<RelationshipTemplateId>> GetDatabaseIdsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken);
    }
}

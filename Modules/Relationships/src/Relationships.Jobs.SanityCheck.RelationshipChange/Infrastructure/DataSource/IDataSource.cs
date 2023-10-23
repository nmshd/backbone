using Backbone.Relationships.Domain.Ids;

namespace Backbone.Relationships.Jobs.SanityCheck.RelationshipChange.Infrastructure.DataSource;

public interface IDataSource
{
    Task<IEnumerable<RelationshipChangeId>> GetDatabaseIdsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken);
}

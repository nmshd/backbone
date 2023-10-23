using Backbone.Relationships.Domain.Ids;

namespace Backbone.Relationships.Jobs.SanityCheck.RelationshipTemplate.Infrastructure.DataSource;

public interface IDataSource
{
    Task<IEnumerable<RelationshipTemplateId>> GetDatabaseIdsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken);
}

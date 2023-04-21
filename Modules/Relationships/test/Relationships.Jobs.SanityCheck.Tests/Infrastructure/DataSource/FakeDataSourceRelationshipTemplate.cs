using Backbone.Modules.Relationships.Domain.Ids;
using Relationships.Jobs.SanityCheck.RelationshipTemplate.Infrastructure.DataSource;

namespace Relationships.Jobs.SanityCheck.Tests.Infrastructure.DataSource;

public class FakeDataSourceRelationshipTemplate : IDataSource
{
    public List<RelationshipTemplateId> DatabaseIds { get; } = new();
    public List<string> BlobIds { get; } = new();

    public Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(BlobIds as IEnumerable<string>);
    }

    public Task<IEnumerable<RelationshipTemplateId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(DatabaseIds as IEnumerable<RelationshipTemplateId>);
    }
}
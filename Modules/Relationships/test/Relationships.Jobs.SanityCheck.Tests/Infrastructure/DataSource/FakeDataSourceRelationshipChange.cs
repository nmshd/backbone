using Backbone.Modules.Relationships.Domain.Ids;
using Relationships.Jobs.SanityCheck.RelationshipChange.Infrastructure.DataSource;

namespace Relationships.Jobs.SanityCheck.Tests.Infrastructure.DataSource
{
    public class FakeDataSourceRelationshipChange : IDataSource
    {
        public List<RelationshipChangeId> DatabaseIds { get; } = new();
        public List<string> BlobIds { get; } = new();

        public Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(BlobIds as IEnumerable<string>);
        }

        public Task<IEnumerable<RelationshipChangeId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(DatabaseIds as IEnumerable<RelationshipChangeId>);
        }
    }
}
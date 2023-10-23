using Backbone.Messages.Domain.Ids;
using Backbone.Messages.Jobs.SanityCheck.Infrastructure.DataSource;

namespace Backbone.Messages.Jobs.SanityCheck.Tests.Infrastructure.DataSource;

public class FakeDataSource : IDataSource
{
    public List<MessageId> DatabaseIds { get; } = new();
    public List<string> BlobIds { get; } = new();

    public Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(BlobIds as IEnumerable<string>);
    }

    public Task<IEnumerable<MessageId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(DatabaseIds as IEnumerable<MessageId>);
    }
}

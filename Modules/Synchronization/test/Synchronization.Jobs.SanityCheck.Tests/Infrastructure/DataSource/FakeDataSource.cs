using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Jobs.SanityCheck.Infrastructure.DataSource;

namespace Backbone.Modules.Synchronization.Jobs.SanityCheck.Tests.Infrastructure.DataSource;

public class FakeDataSource : IDataSource
{
    public List<DatawalletModificationId> DatabaseIds { get; } = new();
    public List<string> BlobIds { get; } = new();

    public Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(BlobIds as IEnumerable<string>);
    }

    public Task<IEnumerable<DatawalletModificationId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(DatabaseIds as IEnumerable<DatawalletModificationId>);
    }
}

using Backbone.Synchronization.Domain.Entities;

namespace Backbone.Synchronization.Jobs.SanityCheck.Infrastructure.DataSource;

public interface IDataSource
{
    Task<IEnumerable<DatawalletModificationId>> GetDatabaseIdsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken);
}

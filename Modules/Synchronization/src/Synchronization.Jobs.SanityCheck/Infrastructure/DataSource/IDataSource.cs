using Backbone.Modules.Synchronization.Domain.Entities;

namespace Synchronization.Jobs.SanityCheck.Infrastructure.DataSource;

public interface IDataSource
{
    Task<IEnumerable<DatawalletModificationId>> GetDatabaseIdsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken);
}

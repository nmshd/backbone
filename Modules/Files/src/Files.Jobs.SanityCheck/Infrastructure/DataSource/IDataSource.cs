using Backbone.Files.Domain.Entities;

namespace Backbone.Files.Jobs.SanityCheck.Infrastructure.DataSource;

public interface IDataSource
{
    Task<IEnumerable<FileId>> GetDatabaseIdsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken);
}

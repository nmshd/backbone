using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.Modules.Files.Jobs.SanityCheck.Infrastructure.DataSource;

public interface IDataSource
{
    Task<IEnumerable<FileId>> GetDatabaseIdsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken);
}

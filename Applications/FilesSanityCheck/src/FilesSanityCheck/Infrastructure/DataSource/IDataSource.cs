using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.FilesSanityCheck.Infrastructure.DataSource;

public interface IDataSource
{
    Task<IEnumerable<FileId>> GetDatabaseIdsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken);
}

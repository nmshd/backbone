using Backbone.FilesSanityCheck.Infrastructure.DataSource;
using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.FilesSanityCheck.Tests.Infrastructure.DataSource;

public class FakeDataSource : IDataSource
{
    public List<FileId> DatabaseIds { get; } = [];
    public List<string> BlobIds { get; } = [];

    public Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<string>>(BlobIds);
    }

    public Task<IEnumerable<FileId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<FileId>>(DatabaseIds);
    }
}

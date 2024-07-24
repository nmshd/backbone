using Backbone.FilesSanityCheck.Infrastructure.DataSource;
using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.FilesSanityCheck.Tests.Infrastructure.DataSource;

public class FakeDataSource : IDataSource
{
    public List<FileId> DatabaseIds { get; } = [];
    public List<string> BlobIds { get; } = [];

    public Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(BlobIds as IEnumerable<string>);
    }

    public Task<IEnumerable<FileId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(DatabaseIds as IEnumerable<FileId>);
    }
}

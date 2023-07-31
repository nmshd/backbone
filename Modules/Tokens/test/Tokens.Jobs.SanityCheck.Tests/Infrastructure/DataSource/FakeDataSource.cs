using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Jobs.SanityCheck.Infrastructure.DataSource;

namespace Tokens.Jobs.SanityCheck.Tests.Infrastructure.DataSource;

public class FakeDataSource : IDataSource
{
    public List<TokenId> DatabaseIds { get; } = new();
    public List<string> BlobIds { get; } = new();

    public Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(BlobIds as IEnumerable<string>);
    }

    public Task<IEnumerable<TokenId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(DatabaseIds as IEnumerable<TokenId>);
    }
}

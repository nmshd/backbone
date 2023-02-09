using Backbone.Modules.Tokens.Domain.Entities;

namespace Tokens.Jobs.SanityCheck.Infrastructure.DataSource
{
    public interface IDataSource
    {
        Task<IEnumerable<TokenId>> GetDatabaseIdsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken);
    }
}

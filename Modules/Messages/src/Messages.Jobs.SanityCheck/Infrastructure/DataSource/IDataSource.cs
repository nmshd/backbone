using Backbone.Messages.Domain.Ids;

namespace Backbone.Messages.Jobs.SanityCheck.Infrastructure.DataSource;

public interface IDataSource
{
    Task<IEnumerable<MessageId>> GetDatabaseIdsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken);
}

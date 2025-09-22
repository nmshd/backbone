namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface IFilesRepository
{
    Task<uint> Count(string owner, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);

    Task<long> AggregateUsedSpace(string owner, DateTime from, DateTime to, CancellationToken cancellationToken);
}

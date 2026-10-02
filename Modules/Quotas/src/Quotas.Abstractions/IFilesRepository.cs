namespace Backbone.Modules.Quotas.Abstractions;

public interface IFilesRepository
{
    Task<uint> Count(string owner, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);

    Task<long> AggregateUsedSpace(string owner, DateTime from, DateTime to, CancellationToken cancellationToken);
}

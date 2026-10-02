namespace Backbone.Modules.Quotas.Abstractions;

public interface IRelationshipsRepository
{
    Task<uint> Count(string participant, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);
}

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface IRelationshipsRepository
{
    Task<uint> Count(string participant, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);
}

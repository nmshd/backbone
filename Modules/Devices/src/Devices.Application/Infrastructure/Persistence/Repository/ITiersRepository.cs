using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface ITiersRepository
{
    Task AddAsync(Tier tier, CancellationToken cancellationToken);
    Task<bool> ExistsWithId(TierId tierId, CancellationToken cancellationToken);
    Task<bool> ExistsWithName(TierName tierName, CancellationToken cancellationToken);
    Task<DbPaginationResult<Tier>> List(PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task<Tier?> GetBasicTier(CancellationToken cancellationToken);
    Task<Tier?> Get(TierId tierId, CancellationToken cancellationToken);
    Task<Tier?> Get(TierName tierName, CancellationToken cancellationToken, bool track = false);
    Task Remove(Tier tier);
    Task<int> GetNumberOfIdentitiesAssignedToTier(Tier tier, CancellationToken cancellationToken);
    Task<int> GetNumberOfClientsWithDefaultTier(Tier tier, CancellationToken cancellationToken);
    Task<IEnumerable<Tier>> ListByIds(IEnumerable<TierId> ids, CancellationToken cancellationToken);
}

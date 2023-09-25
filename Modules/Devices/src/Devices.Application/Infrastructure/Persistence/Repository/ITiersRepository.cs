using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface ITiersRepository
{
    Task AddAsync(Tier tier, CancellationToken cancellationToken);
    Task<bool> ExistsWithId(TierId tierId, CancellationToken cancellationToken);
    Task<bool> ExistsWithName(TierName tierName, CancellationToken cancellationToken);
    Task<DbPaginationResult<Tier>> FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task<Tier> GetBasicTierAsync(CancellationToken cancellationToken);
    Task<Tier> FindById(TierId tierId, CancellationToken cancellationToken);
    Task<Tier> FindByName(TierName tierName, CancellationToken cancellationToken, bool track = false);
    Task Remove(Tier tier);
    Task<int> GetNumberOfIdentitiesAssignedToTier(Tier tier, CancellationToken cancellationToken);
    Task<int> GetNumberOfClientsWithDefaultTier(Tier tier, CancellationToken cancellationToken);
}

using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Tiers.Queries.ListTiers;
public class FindAllStubRepository : ITiersRepository
{
    private readonly DbPaginationResult<Tier> _tiers;

    public FindAllStubRepository(DbPaginationResult<Tier> tiers)
    {
        _tiers = tiers;
    }

    public Task AddAsync(Tier tier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsWithId(TierId tierId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsWithName(TierName tierName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<DbPaginationResult<Tier>> FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        return Task.FromResult(_tiers);
    }

    public Task<Tier?> FindById(TierId tierId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Tier?> FindByName(TierName tierName, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Tier>> FindByIds(IEnumerable<TierId> tiers, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Tier?> FindBasicTier(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetNumberOfClientsWithDefaultTier(Tier tier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetNumberOfIdentitiesAssignedToTier(Tier tier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Remove(Tier tier)
    {
        throw new NotImplementedException();
    }

}

using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;

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

    public Task<DbPaginationResult<Tier>> FindAll(PaginationFilter paginationFilter)
    {
        return Task.FromResult(_tiers);
    }

    public Task<Tier> GetBasicTierAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

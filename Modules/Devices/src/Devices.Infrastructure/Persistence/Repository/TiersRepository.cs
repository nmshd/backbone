using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;
using Dapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class TiersRepository : ITiersRepository
{
    private readonly DbSet<Tier> _tiersDbSet;
    private readonly DevicesDbContext _dbContext;

    public TiersRepository(DevicesDbContext dbContext)
    {
        _dbContext = dbContext;
        _tiersDbSet = dbContext.Set<Tier>();
    }

    public async Task AddAsync(Tier tier, CancellationToken cancellationToken)
    {
        await _tiersDbSet.AddAsync(tier, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Remove(Tier tier)
    {
        _tiersDbSet.Remove(tier);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsWithName(TierName tierName, CancellationToken cancellationToken)
    {
        return await _tiersDbSet.AnyAsync(t => t.Name == tierName, cancellationToken);
    }

    public async Task<DbPaginationResult<Tier>> FindAll(PaginationFilter paginationFilter)
    {
        var paginationResult = await _tiersDbSet
            .OrderAndPaginate(d => d.Name, paginationFilter);
        return paginationResult;
    }

    public async Task<Tier> FindById(TierId tierId, CancellationToken cancellationToken)
    {
        var dto = await _tiersDbSet
            .Select(t => new { Tier = t, Addresses = t.Identities.Select(i => i.Address) })
            .FirstOrDefaultAsync(dto => dto.Tier.Id == tierId, cancellationToken) ?? throw new NotFoundException(nameof(Tier));

        var tier = dto.Tier;
        tier.IdentityAddresses = dto.Addresses.AsList();

        return tier;
    }

    public async Task<Tier> GetBasicTierAsync(CancellationToken cancellationToken)
    {
        return await _tiersDbSet.GetBasicTier(cancellationToken);
    }
}

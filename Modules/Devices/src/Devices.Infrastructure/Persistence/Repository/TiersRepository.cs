using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Core;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class TiersRepository : ITiersRepository
{
    private readonly IQueryable<Tier> _readonlyTiers;
    private readonly DbSet<Tier> _tiers;
    private readonly DevicesDbContext _dbContext;
    private readonly OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> _applicationManager;

    public TiersRepository(DevicesDbContext dbContext, OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> applicationManager)
    {
        _dbContext = dbContext;
        _tiers = dbContext.Set<Tier>();
        _readonlyTiers = dbContext.Set<Tier>().AsNoTracking();
        _applicationManager = applicationManager;
    }

    public async Task AddAsync(Tier tier, CancellationToken cancellationToken)
    {
        await _tiers.AddAsync(tier, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Tier?> Get(TierName tierName, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _tiers : _readonlyTiers).FirstOrDefaultAsync(i => i.Name == tierName, cancellationToken);
    }

    public async Task Remove(Tier tier)
    {
        _tiers.Remove(tier);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> GetNumberOfIdentitiesAssignedToTier(Tier tier, CancellationToken cancellationToken)
    {
        return await _dbContext.Identities.CountAsync(i => i.TierId == tier.Id, cancellationToken);
    }

    public async Task<int> GetNumberOfClientsWithDefaultTier(Tier tier, CancellationToken cancellationToken)
    {
        return (int)await _applicationManager.CountAsync(clients => clients.Where(client => client.DefaultTier == tier.Id), cancellationToken);
    }

    public async Task<bool> ExistsWithId(TierId tierId, CancellationToken cancellationToken)
    {
        return await _tiers.AnyAsync(t => t.Id == tierId, cancellationToken);
    }

    public async Task<bool> ExistsWithName(TierName tierName, CancellationToken cancellationToken)
    {
        return await _tiers.AnyAsync(t => t.Name == tierName, cancellationToken);
    }

    public async Task<DbPaginationResult<Tier>> List(PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        var paginationResult = await _tiers
            .OrderAndPaginate(d => d.Name, paginationFilter, cancellationToken);
        return paginationResult;
    }

    public async Task<Tier?> Get(TierId tierId, CancellationToken cancellationToken)
    {
        return await _tiers.FirstOrDefaultAsync(t => t.Id == tierId, cancellationToken) ?? throw new NotFoundException(nameof(Tier));
    }

    public async Task<Tier?> GetBasicTier(CancellationToken cancellationToken)
    {
        return await _tiers.GetBasicTier(cancellationToken);
    }

    public async Task<IEnumerable<Tier>> ListByIds(IEnumerable<TierId> tierIds, CancellationToken cancellationToken)
    {
        return await _tiers.Where(t => tierIds.Contains(t.Id)).ToListAsync(cancellationToken);
    }
}

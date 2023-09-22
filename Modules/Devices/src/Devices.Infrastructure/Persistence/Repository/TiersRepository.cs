using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Core;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class TiersRepository : ITiersRepository
{
    private readonly DbSet<Tier> _tiersDbSet;
    private readonly DevicesDbContext _dbContext;
    private readonly OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> _applicationManager;

    public TiersRepository(DevicesDbContext dbContext, OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> applicationManager)
    {
        _dbContext = dbContext;
        _tiersDbSet = dbContext.Set<Tier>();
        _applicationManager = applicationManager;
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
        return await _tiersDbSet.AnyAsync(t => t.Id == tierId, cancellationToken);
    }

    public async Task<bool> ExistsWithName(TierName tierName, CancellationToken cancellationToken)
    {
        return await _tiersDbSet.AnyAsync(t => t.Name == tierName, cancellationToken);
    }

    public async Task<DbPaginationResult<Tier>> FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        var paginationResult = await _tiersDbSet
            .OrderAndPaginate(d => d.Name, paginationFilter, cancellationToken);
        return paginationResult;
    }

    public async Task<Tier> FindById(TierId tierId, CancellationToken cancellationToken)
    {
        return await _tiersDbSet.FirstOrDefaultAsync(t => t.Id == tierId, cancellationToken) ?? throw new NotFoundException(nameof(Tier));
    }

    public async Task<Tier> GetBasicTierAsync(CancellationToken cancellationToken)
    {
        return await _tiersDbSet.GetBasicTier(cancellationToken);
    }
}

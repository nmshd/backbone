using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;

public class TiersRepository : ITiersRepository
{
    private readonly DbSet<Tier> _tiers;
    private readonly IQueryable<Tier> _readOnlyTiers;
    private readonly IQueryable<TierQuotaDefinition> _readOnlyTierQuotaDefinitions;
    private readonly DbSet<TierQuotaDefinition> _tierQuotaDefinitions;
    private readonly QuotasDbContext _dbContext;

    public TiersRepository(QuotasDbContext dbContext)
    {
        _dbContext = dbContext;
        _tiers = dbContext.Set<Tier>();
        _readOnlyTiers = dbContext.Tiers.AsNoTracking();
        _tierQuotaDefinitions = dbContext.Set<TierQuotaDefinition>();
        _readOnlyTierQuotaDefinitions = dbContext.Set<TierQuotaDefinition>().AsNoTracking();
    }

    public async Task Add(Tier tier, CancellationToken cancellationToken)
    {
        await _tiers.AddAsync(tier, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Tier?> Find(string id, CancellationToken cancellationToken, bool track = false)
    {
        var tier = await (track ? _tiers : _readOnlyTiers)
            .IncludeAll(_dbContext)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        return tier;
    }

    public async Task<TierQuotaDefinition> FindTierQuotaDefinition(string id, CancellationToken cancellationToken, bool track = false)
    {
        var tierQuotaDefinition = await (track ? _tierQuotaDefinitions : _readOnlyTierQuotaDefinitions)
            .IncludeAll(_dbContext)
            .FirstWithId(id, cancellationToken);

        return tierQuotaDefinition;
    }

    public async Task RemoveById(TierId tierId)
    {
        await _tiers.Where(t => t.Id == tierId).ExecuteDeleteAsync();
    }

    public async Task RemoveTierQuotaDefinitionIfOrphaned(TierQuotaDefinitionId tierQuotaDefinitionId)
    {
        var tierQuotaDefinition = await _tierQuotaDefinitions.FirstWithId(tierQuotaDefinitionId, CancellationToken.None);

        if (_dbContext.Entry(tierQuotaDefinition).Property("TierId").CurrentValue == null)
            await _tierQuotaDefinitions.Where(t => t.Id == tierQuotaDefinitionId).ExecuteDeleteAsync();
    }

    public async Task Update(Tier tier, CancellationToken cancellationToken)
    {
        _dbContext.RemoveRange(_dbContext.ChangeTracker
            .Entries<TierQuotaDefinition>()
            .Where(e => e.State == EntityState.Modified)
            .Select(e => e.Entity)
            .ToList());

        _tiers.Update(tier);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

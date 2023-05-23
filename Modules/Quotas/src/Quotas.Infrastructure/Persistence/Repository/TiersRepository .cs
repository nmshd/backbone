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
    private readonly QuotasDbContext _dbContext;

    public TiersRepository(QuotasDbContext dbContext)
    {
        _dbContext = dbContext;
        _tiers = dbContext.Set<Tier>();
    }

    public async Task Add(Tier tier, CancellationToken cancellationToken)
    {
        await _tiers.AddAsync(tier, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Tier> Find(string id, CancellationToken cancellationToken)
    {
        var tier = await _tiers.FirstWithId(id, cancellationToken);
        return tier;
    }

    public async Task Update(Tier tier, CancellationToken cancellationToken)
    {
        _tiers.Update(tier);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
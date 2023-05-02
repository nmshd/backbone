using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
public class TiersRepository : ITiersRepository
{
    private readonly DbSet<Tier> _tiersDbSet;
    private readonly QuotasDbContext _dbContext;

    public TiersRepository(QuotasDbContext dbContext)
    {
        _dbContext = dbContext;
        _tiersDbSet = dbContext.Set<Tier>();
    }

    public async Task Add(Tier tier, CancellationToken cancellationToken)
    {
        await _tiersDbSet.AddAsync(tier, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
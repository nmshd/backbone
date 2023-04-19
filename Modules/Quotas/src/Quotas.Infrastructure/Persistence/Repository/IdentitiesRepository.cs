using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Entities;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
public class IdentitiesRepository : IIdentitiesRepository
{
    private readonly DbSet<Identity> _identityDbSet;
    private readonly QuotasDbContext _dbContext;

    public IdentitiesRepository(QuotasDbContext dbContext)
    {
        _dbContext = dbContext;
        _identityDbSet = dbContext.Set<Identity>();
    }

    public async Task Add(Identity identity, CancellationToken cancellationToken)
    {
        await _identityDbSet.AddAsync(identity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

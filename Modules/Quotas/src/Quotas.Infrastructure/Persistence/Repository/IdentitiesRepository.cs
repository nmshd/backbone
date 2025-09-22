using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;

public class IdentitiesRepository : IIdentitiesRepository
{
    private readonly DbSet<Identity> _identitiesDbSet;
    private readonly IQueryable<Identity> _readOnlyIdentities;
    private readonly QuotasDbContext _dbContext;

    public IdentitiesRepository(QuotasDbContext dbContext)
    {
        _dbContext = dbContext;
        _identitiesDbSet = dbContext.Set<Identity>();
        _readOnlyIdentities = dbContext.Identities.AsNoTracking();
    }

    public async Task Add(Identity identity, CancellationToken cancellationToken)
    {
        await _identitiesDbSet.AddAsync(identity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(Expression<Func<Identity, bool>> filter, CancellationToken cancellationToken)
    {
        await _identitiesDbSet.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<Identity?> Get(string address, CancellationToken cancellationToken, bool track = false)
    {
        var identity = await (track ? _identitiesDbSet : _readOnlyIdentities)
            .IncludeAll(_dbContext)
            .AsSplitQuery()
            .FirstOrDefaultAsync(i => i.Address == address, cancellationToken);

        return identity;
    }

    public async Task<IEnumerable<Identity>> ListByAddresses(IReadOnlyCollection<string> identityAddresses, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _identitiesDbSet : _readOnlyIdentities)
            .Where(i => identityAddresses.Contains(i.Address))
            .IncludeAll(_dbContext)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Identity>> ListWithTier(TierId tierId, CancellationToken cancellationToken, bool track = false)
    {
        var identities = await (track ? _identitiesDbSet : _readOnlyIdentities)
            .IncludeAll(_dbContext)
            .AsSplitQuery()
            .Where(i => i.TierId == tierId)
            .ToListAsync(cancellationToken);

        return identities;
    }

    public async Task Update(IEnumerable<Identity> identities, CancellationToken cancellationToken)
    {
        _dbContext.UpdateRange(identities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Identity identity, CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

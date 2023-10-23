using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Quotas.Domain.Aggregates.Identities;
using Backbone.Quotas.Domain.Aggregates.Tiers;
using Backbone.Quotas.Infrastructure.Persistence.Database;
using Backbone.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Quotas.Infrastructure.Persistence.Repository;

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

    public async Task<Identity> Find(string address, CancellationToken cancellationToken, bool track = false)
    {
        var identity = await (track ? _identitiesDbSet : _readOnlyIdentities)
            .IncludeAll(_dbContext)
            .FirstWithAddress(address, cancellationToken);

        return identity;
    }

    public async Task<IEnumerable<Identity>> FindByAddresses(IReadOnlyCollection<string> identityAddresses, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _identitiesDbSet : _readOnlyIdentities)
            .Where(i => identityAddresses.Contains(i.Address))
            .IncludeAll(_dbContext)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Identity>> FindWithTier(TierId tierId, CancellationToken cancellationToken, bool track = false)
    {
        var identities = await (track ? _identitiesDbSet : _readOnlyIdentities)
            .IncludeAll(_dbContext)
            .WithTier(tierId, cancellationToken);

        return identities;
    }

    public async Task Update(IEnumerable<Identity> identities, CancellationToken cancellationToken)
    {
        _dbContext.UpdateRange(identities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Identity identity, CancellationToken cancellationToken)
    {
        _dbContext.Update(identity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

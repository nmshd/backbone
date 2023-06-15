using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Extensions;
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

    public async Task<IEnumerable<Identity>> FindByAddresses(IReadOnlyCollection<string> identityAddresses, CancellationToken cancellationToken)
    {
        return await _identitiesDbSet
            .Where(i => identityAddresses.Contains(i.Address))
            .Include(i => i.TierQuotas)
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
        await _dbContext.SaveChangesAsync();
    }
}

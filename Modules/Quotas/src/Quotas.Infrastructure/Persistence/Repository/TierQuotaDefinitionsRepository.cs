using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;

public class TierQuotaDefinitionsRepository : ITierQuotaDefinitionsRepository
{
    private readonly IQueryable<TierQuotaDefinition> _readOnlyTierQuotaDefinitions;

    public TierQuotaDefinitionsRepository(QuotasDbContext dbContext)
    {
        _readOnlyTierQuotaDefinitions = dbContext.Set<TierQuotaDefinition>();
    }

    public Task<TierQuotaDefinition> Find(string id, CancellationToken cancellationToken)
    {
        var tierQuotaDefiniton = _readOnlyTierQuotaDefinitions.FirstWithId(id, cancellationToken);
        return tierQuotaDefiniton;
    }
}
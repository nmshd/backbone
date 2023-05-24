using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;

public class FindTierQuotaDefinitionsStubRepository : ITierQuotaDefinitionsRepository
{
    private readonly TierQuotaDefinition _tierQuotaDefinition;

    public FindTierQuotaDefinitionsStubRepository(TierQuotaDefinition tierQuotaDefinition)
    {
        _tierQuotaDefinition = tierQuotaDefinition;
    }

    public Task<TierQuotaDefinition> Find(string id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_tierQuotaDefinition);
    }
}

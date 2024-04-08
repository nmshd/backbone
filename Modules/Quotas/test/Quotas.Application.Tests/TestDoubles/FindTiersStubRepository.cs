using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.TestDoubles;

public class FindTiersStubRepository : ITiersRepository
{
    private readonly Tier _tier;

    public FindTiersStubRepository(Tier tier)
    {
        _tier = tier;
    }

    public Task Add(Tier tier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Tier?> Find(string id, CancellationToken cancellationToken, bool track = false)
    {
        return Task.FromResult((Tier?)_tier);
    }

    public Task<TierQuotaDefinition> FindTierQuotaDefinition(string id, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }

    public Task RemoveById(TierId tierId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveTierQuotaDefinitionIfOrphaned(TierQuotaDefinitionId tierQuotaDefinitionId)
    {
        throw new NotImplementedException();
    }

    public Task Update(Tier tier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

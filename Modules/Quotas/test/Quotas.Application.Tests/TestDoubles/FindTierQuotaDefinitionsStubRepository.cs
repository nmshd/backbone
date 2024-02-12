using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.TestDoubles;

public class FindTierQuotaDefinitionsStubRepository : ITiersRepository
{
    private readonly TierQuotaDefinition _tierQuotaDefinition;

    public FindTierQuotaDefinitionsStubRepository(TierQuotaDefinition tierQuotaDefinition)
    {
        _tierQuotaDefinition = tierQuotaDefinition;
    }

    public Task Add(Tier tier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Tier?> Find(string id, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }

    public Task<TierQuotaDefinition> FindTierQuotaDefinition(string id, CancellationToken cancellationToken, bool track = false)
    {
        return Task.FromResult(_tierQuotaDefinition);
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

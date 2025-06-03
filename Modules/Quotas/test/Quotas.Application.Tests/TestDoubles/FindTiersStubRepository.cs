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
        throw new NotSupportedException();
    }

    public Task<Tier?> Get(string id, CancellationToken cancellationToken, bool track = false)
    {
        return Task.FromResult<Tier?>(_tier);
    }

    public Task<TierQuotaDefinition> GetTierQuotaDefinition(string id, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotSupportedException();
    }

    public Task RemoveById(TierId tierId)
    {
        throw new NotSupportedException();
    }

    public Task Update(Tier tier, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}

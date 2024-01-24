using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.TestDoubles;
public class AddMockTiersRepository : ITiersRepository
{
    public bool WasCalled { get; private set; }
    public Tier WasCalledWith { get; private set; } = null!;

    public Task Add(Tier tier, CancellationToken cancellationToken)
    {
        WasCalled = true;
        WasCalledWith = tier;
        return Task.CompletedTask;
    }

    public Task<Tier> Find(string id, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }

    public Task<TierQuotaDefinition> FindTierQuotaDefinition(string id, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }

    public Task RemoveById(TierId tierId)
    {
        throw new NotImplementedException();
    }

    public Task Update(Tier tier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Quotas.CreateQuotaForTier;

public class MockTiersRepository : ITiersRepository
{
    private readonly List<Tier> _tiers;
    public bool WasUpdateCalled { get; private set; }
    public Tier? WasUpdateCalledWith { get; private set; }

    public MockTiersRepository(List<Tier> tiers)
    {
        _tiers = tiers;
    }

    public Task Add(Tier tier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Tier> Find(string id, CancellationToken cancellationToken)
    {
        var tier = _tiers.FirstOrDefault(t => t.Id == id);

        if (tier == null)
            throw new NotFoundException(nameof(Tier));

        return Task.FromResult(tier);
    }

    public Task Update(Tier tier, CancellationToken cancellationToken)
    {
        WasUpdateCalled = true;
        WasUpdateCalledWith = tier;
        return Task.CompletedTask;
    }
}

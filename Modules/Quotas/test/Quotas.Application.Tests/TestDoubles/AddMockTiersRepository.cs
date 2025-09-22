using System.Diagnostics.CodeAnalysis;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.TestDoubles;

public class AddMockTiersRepository : ITiersRepository
{
    [MemberNotNullWhen(true, nameof(WasCalledWith))]
    public bool WasCalled => WasCalledWith != null;

    public Tier? WasCalledWith { get; private set; }

    public Task Add(Tier tier, CancellationToken cancellationToken)
    {
        WasCalledWith = tier;
        return Task.CompletedTask;
    }

    public Task<Tier?> Get(string id, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotSupportedException();
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

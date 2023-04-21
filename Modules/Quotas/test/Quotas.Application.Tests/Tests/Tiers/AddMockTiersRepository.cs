using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Tiers;
public class AddMockTiersRepository : ITierRepository
{
    public bool WasCalled { get; private set; }
    public Tier? WasCalledWith { get; private set; }

    public Task Add(Tier tier, CancellationToken cancellationToken)
    {
        WasCalled = true;
        WasCalledWith = tier;
        return Task.FromResult(WasCalledWith);
    }
}

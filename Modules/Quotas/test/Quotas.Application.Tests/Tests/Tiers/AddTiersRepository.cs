using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Tiers;
public class AddTiersRepository : ITierRepository
{
    private readonly Tier _tiers;

    public AddTiersRepository(Tier tiers)
    {
        _tiers = tiers;
    }

    public Task Add(Tier tiers, CancellationToken cancellationToken)
    {
        return Task.FromResult(tiers);
    }
}

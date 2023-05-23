using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;

public class FindMockTiersRepository : ITiersRepository
{
    private readonly List<Tier> _tiers;

    public FindMockTiersRepository(List<Tier> tiers)
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
        return Task.FromResult(tier);
    }

    public Task Update(Tier tier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

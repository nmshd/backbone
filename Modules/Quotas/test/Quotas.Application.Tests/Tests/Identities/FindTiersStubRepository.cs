using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;

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

    public Task<Tier> Find(string id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_tier);
    }

    public Task Update(Tier tier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

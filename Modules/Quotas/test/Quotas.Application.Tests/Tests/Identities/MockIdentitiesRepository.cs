using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;
public class MockIdentitiesRepository : IIdentitiesRepository
{
    private readonly List<Identity> _identities;

    public MockIdentitiesRepository()
    {
        _identities = new();
    }

    public MockIdentitiesRepository(List<Identity> identities)
    {
        _identities = identities;
    }

    public bool WasAddCalled { get; private set; }
    public Identity? WasAddCalledWith { get; private set; }
    public bool WasUpdateFromRangeCalled { get; private set; }
    public IEnumerable<Identity>? WasUpdateFromRangeCalledWith { get; private set; }

    public Task Add(Identity identity, CancellationToken cancellationToken)
    {
        WasAddCalled = true;
        WasAddCalledWith = identity;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Identity>> FindWithTier(TierId tierId, CancellationToken cancellationToken, bool track = false)
    {
        var identities = _identities.Where(identity => identity.TierId == tierId);
        return Task.FromResult(identities);
    }

    public Task Update(IEnumerable<Identity> identities, CancellationToken cancellationToken)
    {
        WasUpdateFromRangeCalled = true;
        WasUpdateFromRangeCalledWith = identities;
        return Task.CompletedTask;

    }
}

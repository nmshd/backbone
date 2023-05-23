using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

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
    public bool WasUpdateCalled { get; private set; }
    public Identity? WasUpdateCalledWith { get; private set; }
    public bool WasUpdateFromRangeCalled { get; private set; }
    public IEnumerable<Identity>? WasUpdateFromRangeCalledWith { get; private set; }

    public Task Add(Identity identity, CancellationToken cancellationToken)
    {
        WasAddCalled = true;
        WasAddCalledWith = identity;
        return Task.CompletedTask;
    }

    public IEnumerable<Identity> FindWithTier(string tierId)
    {
        var identities = _identities.Where(identity => identity.TierId == tierId);
        return identities;
    }

    public Task Update(Identity identity, CancellationToken cancellationToken)
    {
        WasUpdateCalled = true;
        WasUpdateCalledWith = identity;
        return Task.CompletedTask;
    }

    public Task Update(IEnumerable<Identity> identities, CancellationToken cancellationToken)
    {
        WasUpdateFromRangeCalled = true;
        WasUpdateFromRangeCalledWith = identities;
        return Task.CompletedTask;

    }
}

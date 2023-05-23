using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;
public class AddMockIdentitiesRepository : IIdentitiesRepository
{
    public bool WasCalled { get; private set; }
    public Identity? WasCalledWith { get; private set; }

    public Task Add(Identity identity, CancellationToken cancellationToken)
    {
        WasCalled = true;
        WasCalledWith = identity;
        return Task.CompletedTask;
    }

    public IEnumerable<Identity> FindWithTier(string tierId)
    {
        throw new NotImplementedException();
    }

    public Task Update(Identity identity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(IEnumerable<Identity> identities, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Entities;

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
}

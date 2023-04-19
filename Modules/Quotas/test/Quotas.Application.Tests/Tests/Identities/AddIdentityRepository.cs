using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Entities;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;
public class AddIdentityRepository : IIdentitiesRepository
{
    private readonly Identity _identities;
    
    public AddIdentityRepository(Identity identities) 
    {
        _identities = identities;
    }

    public Task Add(Identity identity, CancellationToken cancellationToken)
    {
        return Task.FromResult(_identities);
    }
}

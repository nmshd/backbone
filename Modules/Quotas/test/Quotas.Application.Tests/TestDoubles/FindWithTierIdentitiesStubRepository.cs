using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.TestDoubles;
public class FindWithTierIdentitiesStubRepository : IIdentitiesRepository
{
    private readonly IEnumerable<Identity> _identities;

    public FindWithTierIdentitiesStubRepository(IEnumerable<Identity> identities)
    {
        _identities = identities;
    }

    public Task Add(Identity identity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Identity> Find(string address, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Identity>> FindByAddresses(IReadOnlyCollection<string> identityAddresses, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Identity>> FindWithTier(TierId tierId, CancellationToken cancellationToken, bool track = false)
    {
        return Task.FromResult(_identities);
    }

    public Task Update(IEnumerable<Identity> identities, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(Identity identity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

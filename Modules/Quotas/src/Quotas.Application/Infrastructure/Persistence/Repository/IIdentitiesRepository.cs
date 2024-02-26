using System.Linq.Expressions;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
public interface IIdentitiesRepository
{
    Task Add(Identity identity, CancellationToken cancellationToken);
    Task<IEnumerable<Identity>> FindWithTier(TierId tierId, CancellationToken cancellationToken, bool track = false);
    Task Update(IEnumerable<Identity> identities, CancellationToken cancellationToken);
    Task Update(Identity identity, CancellationToken cancellationToken);
    Task<Identity?> Find(string address, CancellationToken cancellationToken, bool track = false);
    Task<IEnumerable<Identity>> FindByAddresses(IReadOnlyCollection<string> identityAddresses, CancellationToken cancellationToken, bool track = false);
    Task DeleteIdentities(Expression<Func<Identity, bool>> expression, CancellationToken cancellationToken);
}

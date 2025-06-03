using System.Linq.Expressions;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface IIdentitiesRepository
{
    Task Add(Identity identity, CancellationToken cancellationToken);
    Task<IEnumerable<Identity>> ListWithTier(TierId tierId, CancellationToken cancellationToken, bool track = false);
    Task Update(IEnumerable<Identity> identities, CancellationToken cancellationToken);
    Task Update(Identity identity, CancellationToken cancellationToken);
    Task<Identity?> Get(string address, CancellationToken cancellationToken, bool track = false);
    Task<IEnumerable<Identity>> ListByAddresses(IReadOnlyCollection<string> identityAddresses, CancellationToken cancellationToken, bool track = false);
    Task Delete(Expression<Func<Identity, bool>> expression, CancellationToken cancellationToken);
}

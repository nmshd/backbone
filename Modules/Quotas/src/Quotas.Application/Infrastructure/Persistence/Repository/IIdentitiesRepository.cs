using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
public interface IIdentitiesRepository
{
    Task Add(Identity identity, CancellationToken cancellationToken);
    IEnumerable<Identity> FindWithTier(string tierId);
    Task Update(Identity identity, CancellationToken cancellationToken);
    Task Update(IEnumerable<Identity> identities, CancellationToken cancellationToken);
}

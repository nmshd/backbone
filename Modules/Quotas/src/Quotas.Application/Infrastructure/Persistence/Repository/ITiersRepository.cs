using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
public interface ITiersRepository
{
    Task Add(Tier tier, CancellationToken cancellationToken);
    Task<Tier?> Find(string id, CancellationToken cancellationToken, bool track = false);
    Task<TierQuotaDefinition> FindTierQuotaDefinition(string id, CancellationToken cancellationToken, bool track = false);
    Task RemoveById(TierId tierId);
    Task Update(Tier tier, CancellationToken cancellationToken);
}

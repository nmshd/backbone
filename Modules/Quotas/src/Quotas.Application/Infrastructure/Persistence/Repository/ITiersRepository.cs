using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface ITiersRepository
{
    Task Add(Tier tier, CancellationToken cancellationToken);
    Task<Tier?> Get(string id, CancellationToken cancellationToken, bool track = false);
    Task<TierQuotaDefinition> GetTierQuotaDefinition(string id, CancellationToken cancellationToken, bool track = false);
    Task RemoveById(TierId tierId);
    Task Update(Tier tier, CancellationToken cancellationToken);
}

using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface ITierQuotaDefinitionsRepository
{
    Task<TierQuotaDefinition> Find(string id, CancellationToken cancellationToken);
}

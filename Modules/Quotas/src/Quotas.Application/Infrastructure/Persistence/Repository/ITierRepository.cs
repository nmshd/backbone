using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
public interface ITierRepository
{
    Task Add(Tier tier, CancellationToken cancellationToken);
}

using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
public interface ITiersRepository
{
    Task Add(Tier tier, CancellationToken cancellationToken);
}

using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface ITierRepository
{
    Task AddAsync(Tier tier, CancellationToken cancellationToken);
}

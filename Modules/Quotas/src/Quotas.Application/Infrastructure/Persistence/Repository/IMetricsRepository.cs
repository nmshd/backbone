using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface IMetricsRepository
{
    Task<IEnumerable<Metric>> FindAll(CancellationToken cancellationToken);
}
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface IMetricsRepository
{
    Task<IEnumerable<Metric>> FindAll(CancellationToken cancellationToken, bool track = false);
    Task<IEnumerable<Metric>> FindAllWithKeys(IEnumerable<MetricKey> keys, CancellationToken cancellationToken, bool track = false);
    Task<Metric> Find(MetricKey key, CancellationToken cancellationToken, bool track = false);
}

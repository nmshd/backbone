using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Abstractions;

public interface IMetricsRepository
{
    Task<IEnumerable<Metric>> List(CancellationToken cancellationToken, bool track = false);
    Task<IEnumerable<Metric>> List(IEnumerable<MetricKey> keys, CancellationToken cancellationToken, bool track = false);
    Task<Metric> Get(MetricKey key, CancellationToken cancellationToken, bool track = false);
}

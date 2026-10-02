using Backbone.Modules.Quotas.Abstractions;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Module.Tests.TestDoubles;

public class FindMetricsStubRepository : IMetricsRepository
{
    private readonly Metric _metric;

    public FindMetricsStubRepository(Metric metric)
    {
        _metric = metric;
    }

    public Task<Metric> Get(MetricKey key, CancellationToken cancellationToken, bool track = false)
    {
        return Task.FromResult(_metric);
    }

    public Task<IEnumerable<Metric>> List(CancellationToken cancellationToken, bool track = false)
    {
        throw new NotSupportedException();
    }

    public Task<IEnumerable<Metric>> List(IEnumerable<MetricKey> keys, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotSupportedException();
    }
}

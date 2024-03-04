using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.Tests.TestDoubles;
public class FindAllWithKeysMetricsStubRepository : IMetricsRepository
{
    private readonly IEnumerable<Metric> _metrics;

    public FindAllWithKeysMetricsStubRepository(IEnumerable<Metric> metrics)
    {
        _metrics = metrics;
    }

    public Task<Metric> Find(MetricKey key, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Metric>> FindAll(CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Metric>> FindAllWithKeys(IEnumerable<MetricKey> keys, CancellationToken cancellationToken, bool track = false)
    {
        return Task.FromResult(_metrics);
    }
}

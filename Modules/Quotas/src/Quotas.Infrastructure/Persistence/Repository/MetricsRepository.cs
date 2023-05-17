using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
public class MetricsRepository : IMetricsRepository
{
    public Task<List<Metric>> FindAll(CancellationToken cancellationToken)
    {
        var listMetrics = new List<Metric>
        {
            new Metric(MetricKey.NumberOfSentMessages, "NumberOfSentMessages"),
            new Metric(MetricKey.NumberOfRelationships, "NumberOfRelationships"),
            new Metric(MetricKey.NumberOfFiles, "NumberOfFiles"),
            new Metric(MetricKey.FileStorageCapacity, "FileStorageCapacity")
        };

        return Task.FromResult(listMetrics);
    }
}
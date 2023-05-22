using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
public class MetricsRepository : IMetricsRepository
{
    public Task<IEnumerable<Metric>> FindAll(CancellationToken cancellationToken)
    {
        IEnumerable<Metric> metrics = new List<Metric>
        {
            new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages"),
            new Metric(MetricKey.NumberOfRelationships, "Number Of Relationships"),
            new Metric(MetricKey.NumberOfFiles, "Number Of Files"),
            new Metric(MetricKey.FileStorageCapacity, "File Storage Capacity")
        };

        return Task.FromResult(metrics);
    }
}
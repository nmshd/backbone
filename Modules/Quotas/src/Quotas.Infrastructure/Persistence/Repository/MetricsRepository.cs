using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
public class MetricsRepository : IMetricsRepository
{
    public Task<IEnumerable<Metric>> FindAll(CancellationToken cancellationToken)
    {
        var metrics = new List<Metric>
        {
            new Metric(MetricKey.NumberOfSentMessages, "Number of Sent Messages"),
            new Metric(MetricKey.NumberOfRelationships, "Number of Relationships"),
            new Metric(MetricKey.NumberOfFiles, "Number of Files"),
            new Metric(MetricKey.FileStorageCapacity, "File Storage Capacity")
        };

        return Task.FromResult(metrics.AsEnumerable());
    }
}
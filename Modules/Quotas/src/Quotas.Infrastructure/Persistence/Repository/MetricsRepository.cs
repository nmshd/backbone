using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;

public class MetricsRepository : IMetricsRepository
{
    public Task<Metric> Find(MetricKey key, CancellationToken cancellationToken)
    {
        return key switch
        {
            MetricKey.NumberOfSentMessages => Task.FromResult(new Metric(MetricKey.NumberOfSentMessages, "Number of Sent Messages")),
            MetricKey.NumberOfRelationships => Task.FromResult(new Metric(MetricKey.NumberOfRelationships, "Number of Relationships")),
            MetricKey.NumberOfFiles => Task.FromResult(new Metric(MetricKey.NumberOfFiles, "Number of Files")),
            MetricKey.FileStorageCapacity => Task.FromResult(new Metric(MetricKey.FileStorageCapacity, "File Storage Capacity")),
            _ => throw new NotFoundException()
        };
    }

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
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;

public class MetricsRepository : IMetricsRepository
{
    private readonly List<Metric> _metrics;

    public MetricsRepository()
    {
        _metrics =
        [
            new(MetricKey.NumberOfSentMessages, "Number of Sent Messages"),
            new(MetricKey.NumberOfRelationships, "Number of Relationships"),
            new(MetricKey.NumberOfRelationshipTemplates, "Number of Relationship Templates"),
            new(MetricKey.NumberOfFiles, "Number of Files"),
            new(MetricKey.NumberOfTokens, "Number of Tokens"),
            new(MetricKey.UsedFileStorageSpace, "File Storage Capacity (in Megabytes)")
        ];
    }

    public Task<Metric> Find(MetricKey key, CancellationToken cancellationToken, bool track = false)
    {
        var metric = _metrics.FirstOrDefault(metric => metric.Key == key);

        return metric == null ? throw new NotFoundException(nameof(Metric)) : Task.FromResult(metric);
    }

    public Task<IEnumerable<Metric>> FindAll(CancellationToken cancellationToken, bool track = false)
    {
        return Task.FromResult(_metrics.AsEnumerable());
    }

    public Task<IEnumerable<Metric>> FindAllWithKeys(IEnumerable<MetricKey> keys, CancellationToken cancellationToken, bool track = false)
    {
        var metrics = _metrics.Where(m => keys.Contains(m.Key));

        return Task.FromResult(metrics);
    }
}

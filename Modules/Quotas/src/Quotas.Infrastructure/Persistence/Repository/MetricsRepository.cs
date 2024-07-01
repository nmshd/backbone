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
            new Metric(MetricKey.NumberOfSentMessages, "Number of Sent Messages"),
            new Metric(MetricKey.NumberOfRelationships, "Number of Relationships"),
            new Metric(MetricKey.NumberOfRelationshipTemplates, "Number of Relationship Templates"),
            new Metric(MetricKey.NumberOfFiles, "Number of Files"),
            new Metric(MetricKey.NumberOfTokens, "Number of Tokens"),
            new Metric(MetricKey.UsedFileStorageSpace, "File Storage Capacity (in Megabytes)"),
            new Metric(MetricKey.NumberOfStartedDeletionProcesses, "Number of Started Deletion Processes"),
            new Metric(MetricKey.NumberOfCreatedChallenges, "Number of Created Challenges"),
            new Metric(MetricKey.NumberOfCreatedDatawalletModifications, "Number of Created Datawallet Modifications"),
            new Metric(MetricKey.NumberOfCreatedDevices, "Number of Created Devices")
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

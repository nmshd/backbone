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
            new Metric(MetricKey.NUMBER_OF_SENT_MESSAGES, "Number of Sent Messages"),
            new Metric(MetricKey.NUMBER_OF_RELATIONSHIPS, "Number of Relationships"),
            new Metric(MetricKey.NUMBER_OF_RELATIONSHIP_TEMPLATES, "Number of Relationship Templates"),
            new Metric(MetricKey.NUMBER_OF_FILES, "Number of Files"),
            new Metric(MetricKey.NUMBER_OF_TOKENS, "Number of Tokens"),
            new Metric(MetricKey.USED_FILE_STORAGE_SPACE, "File Storage Capacity (in Megabytes)"),
            new Metric(MetricKey.NUMBER_OF_STARTED_DELETION_PROCESSES, "Number of Started Deletion Processes"),
            new Metric(MetricKey.NUMBER_OF_CREATED_CHALLENGES, "Number of Created Challenges"),
            new Metric(MetricKey.NUMBER_OF_CREATED_DATAWALLET_MODIFICATIONS, "Number of Created Datawallet Modifications"),
            new Metric(MetricKey.NUMBER_OF_CREATED_DEVICES, "Number of Created Devices")
        ];
    }

    public Task<Metric> Get(MetricKey key, CancellationToken cancellationToken, bool track = false)
    {
        var metric = _metrics.FirstOrDefault(metric => metric.Key == key);

        return metric == null ? throw new NotFoundException(nameof(Metric)) : Task.FromResult(metric);
    }

    public Task<IEnumerable<Metric>> List(CancellationToken cancellationToken, bool track = false)
    {
        return Task.FromResult(_metrics.AsEnumerable());
    }

    public Task<IEnumerable<Metric>> List(IEnumerable<MetricKey> keys, CancellationToken cancellationToken, bool track = false)
    {
        var metrics = _metrics.Where(m => keys.Contains(m.Key));

        return Task.FromResult(metrics);
    }
}

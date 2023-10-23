using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Quotas.Application.Metrics;
using Backbone.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Quotas.Application.IntegrationEvents.Incoming.FileUploaded;

public class FileUploadedIntegrationEventHandler : IIntegrationEventHandler<FileUploadedIntegrationEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public FileUploadedIntegrationEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(FileUploadedIntegrationEvent @event)
    {
        var identities = new List<string> { @event.Uploader };
        var metrics = new List<string> { MetricKey.NumberOfFiles.Value, MetricKey.UsedFileStorageSpace.Value };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}

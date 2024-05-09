using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.FileUploaded;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.FileUploaded;

public class FileUploadedDomainEventHandler : IDomainEventHandler<FileUploadedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public FileUploadedDomainEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(FileUploadedDomainEvent @event)
    {
        var identities = new List<string> { @event.Uploader };
        var metrics = new List<MetricKey> { MetricKey.NumberOfFiles, MetricKey.UsedFileStorageSpace };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}

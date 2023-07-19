using Backbone.Modules.Quotas.Application.Metrics.Commands.RecalculateMetricStatuses;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.FileUploaded;

public class FileUploadedIntegrationEventHandler : IIntegrationEventHandler<FileUploadedIntegrationEvent>
{
    private readonly IMediator _mediator;

    public FileUploadedIntegrationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(FileUploadedIntegrationEvent @event)
    {
        var identities = new List<string> { @event.Uploader };
        var metrics = new List<string> { MetricKey.NumberOfFiles.Value, MetricKey.UsedFileStorageSpace.Value};

        await _mediator.Send(new RecalculateMetricStatusesCommand(identities, metrics));
    }
}

using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.FileUploaded;
public class FileUploadedIntegrationEvent : IntegrationEvent
{
    public required string FileId { get; set; }
    public required string Uploader { get; set; }
}

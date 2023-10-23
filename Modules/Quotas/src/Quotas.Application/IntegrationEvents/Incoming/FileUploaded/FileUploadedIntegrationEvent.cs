using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Quotas.Application.IntegrationEvents.Incoming.FileUploaded;
public class FileUploadedIntegrationEvent : IntegrationEvent
{
    public string FileId { get; set; }
    public string Uploader { get; set; }
}

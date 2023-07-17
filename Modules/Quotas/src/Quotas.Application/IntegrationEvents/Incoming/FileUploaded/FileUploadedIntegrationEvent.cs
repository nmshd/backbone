using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.FileUploaded;
public class FileUploadedIntegrationEvent : IntegrationEvent
{
    public string FileId { get; private set; }
    public string SenderIdentityAddress { get; private set; }
}

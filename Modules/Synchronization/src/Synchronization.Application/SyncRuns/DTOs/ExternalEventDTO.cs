using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;

public class ExternalEventDTO
{
    public ExternalEventDTO(ExternalEvent externalEvent)
    {
        Id = externalEvent.Id;
        Type = MapExternalEventType(externalEvent.Type);
        Index = externalEvent.Index;
        CreatedAt = externalEvent.CreatedAt;
        SyncErrorCount = externalEvent.SyncErrorCount;
        Payload = externalEvent.Payload;
    }

    public string Id { get; set; }
    public string Type { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
    public byte SyncErrorCount { get; set; }
    public object Payload { get; set; }

    private string MapExternalEventType(ExternalEventType externalEventType)
    {
        return externalEventType switch
        {
            ExternalEventType.MessageReceived => "MessageReceived",
            ExternalEventType.MessageDelivered => "MessageDelivered",

            ExternalEventType.RelationshipStatusChanged => "RelationshipStatusChanged",
            ExternalEventType.RelationshipReactivationRequested => "RelationshipReactivationRequested",
            ExternalEventType.RelationshipReactivationCompleted => "RelationshipReactivationCompleted",

            ExternalEventType.IdentityDeletionProcessStarted => "IdentityDeletionProcessStarted",
            ExternalEventType.IdentityDeletionProcessStatusChanged => "IdentityDeletionProcessStatusChanged",
            ExternalEventType.PeerToBeDeleted => "PeerToBeDeleted",
            ExternalEventType.PeerDeletionCancelled => "PeerDeletionCancelled",
            ExternalEventType.PeerDeleted => "PeerDeleted",

            _ => throw new ArgumentOutOfRangeException(nameof(externalEventType), externalEventType, null)
        };
    }
}

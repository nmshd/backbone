using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class PeerDeletionCancelledExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected PeerDeletionCancelledExternalEvent()
    {
    }

    public PeerDeletionCancelledExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.PeerDeletionCancelled, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string RelationshipId { get; init; }
    }
}

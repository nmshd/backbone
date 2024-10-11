using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class PeerDeletionCancelledExternalEvent : ExternalEvent
{
    public PeerDeletionCancelledExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.PeerDeletionCancelled, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string RelationshipId { get; init; }
    }
}

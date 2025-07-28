using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class PeerDeletionCancelledExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    protected PeerDeletionCancelledExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
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

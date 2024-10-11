using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class PeerDeletedExternalEvent : ExternalEvent
{
    public PeerDeletedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.PeerDeleted, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string RelationshipId { get; init; }
        public required DateTime DeletionDate { get; init; }
    }
}

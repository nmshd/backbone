using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class PeerToBeDeletedExternalEvent : ExternalEvent
{
    public PeerToBeDeletedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.PeerToBeDeleted, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string RelationshipId { get; init; }
        public required DateTime DeletionDate { get; init; }
    }
}

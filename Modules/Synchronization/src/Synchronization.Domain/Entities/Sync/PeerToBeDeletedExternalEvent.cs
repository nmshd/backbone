using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class PeerToBeDeletedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    protected PeerToBeDeletedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
    }

    public PeerToBeDeletedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.PeerToBeDeleted, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string RelationshipId { get; init; }
        public required DateTime DeletionDate { get; init; }
    }
}

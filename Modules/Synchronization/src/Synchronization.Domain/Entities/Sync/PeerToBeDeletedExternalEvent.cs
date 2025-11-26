using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class PeerToBeDeletedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected PeerToBeDeletedExternalEvent()
    {
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

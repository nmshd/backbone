using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class PeerDeletedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected PeerDeletedExternalEvent()
    {
    }

    public PeerDeletedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.PeerDeleted, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string RelationshipId { get; init; }
        public required DateTime DeletionDate { get; init; }
    }
}

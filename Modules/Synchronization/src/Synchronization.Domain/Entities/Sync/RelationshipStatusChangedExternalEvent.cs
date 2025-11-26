using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipStatusChangedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected RelationshipStatusChangedExternalEvent()
    {
    }

    public RelationshipStatusChangedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.RelationshipStatusChanged, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string RelationshipId { get; init; }
    }
}

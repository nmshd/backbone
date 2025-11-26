using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipReactivationCompletedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected RelationshipReactivationCompletedExternalEvent()
    {
    }

    public RelationshipReactivationCompletedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.RelationshipReactivationCompleted, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string RelationshipId { get; init; }
    }
}

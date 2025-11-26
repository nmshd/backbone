using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipReactivationRequestedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected RelationshipReactivationRequestedExternalEvent()
    {
    }

    public RelationshipReactivationRequestedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.RelationshipReactivationRequested, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string RelationshipId { get; init; }
    }
}

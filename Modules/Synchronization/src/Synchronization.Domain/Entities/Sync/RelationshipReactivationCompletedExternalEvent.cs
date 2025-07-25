using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipReactivationCompletedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    protected RelationshipReactivationCompletedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
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

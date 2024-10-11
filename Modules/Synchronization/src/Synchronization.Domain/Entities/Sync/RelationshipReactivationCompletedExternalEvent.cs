using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipReactivationCompletedExternalEvent : ExternalEvent
{
    public RelationshipReactivationCompletedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.RelationshipReactivationCompleted, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string RelationshipId { get; init; }
    }
}

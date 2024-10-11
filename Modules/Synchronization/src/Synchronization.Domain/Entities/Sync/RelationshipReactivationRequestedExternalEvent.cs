using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipReactivationRequestedExternalEvent : ExternalEvent
{
    public RelationshipReactivationRequestedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.RelationshipReactivationRequested, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string RelationshipId { get; init; }
    }
}

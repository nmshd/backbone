using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipStatusChangedExternalEvent : ExternalEvent
{
    public RelationshipStatusChangedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.RelationshipStatusChanged, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string RelationshipId { get; init; }
    }
}

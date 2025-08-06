using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipStatusChangedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    protected RelationshipStatusChangedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
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

using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipReactivationRequestedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    protected RelationshipReactivationRequestedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
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

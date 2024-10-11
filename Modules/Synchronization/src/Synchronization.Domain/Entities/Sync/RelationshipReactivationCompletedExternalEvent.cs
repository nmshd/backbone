using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipReactivationCompletedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    private RelationshipReactivationCompletedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
    }

    public RelationshipReactivationCompletedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.RelationshipReactivationCompleted, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string RelationshipId { get; init; }
    }
}

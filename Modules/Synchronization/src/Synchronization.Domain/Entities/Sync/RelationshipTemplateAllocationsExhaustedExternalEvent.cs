using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipTemplateAllocationsExhaustedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    private RelationshipTemplateAllocationsExhaustedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
    }

    public RelationshipTemplateAllocationsExhaustedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.RelationshipTemplateAllocationsExhausted, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string RelationshipTemplateId { get; init; }
    }
}

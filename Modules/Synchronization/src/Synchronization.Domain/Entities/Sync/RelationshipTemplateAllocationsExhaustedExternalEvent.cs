using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class RelationshipTemplateAllocationsExhaustedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected RelationshipTemplateAllocationsExhaustedExternalEvent()
    {
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

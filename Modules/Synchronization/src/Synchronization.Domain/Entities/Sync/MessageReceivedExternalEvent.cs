using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities.Relationships;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class MessageReceivedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected MessageReceivedExternalEvent()
    {
    }

    public MessageReceivedExternalEvent(IdentityAddress owner, EventPayload payload, RelationshipId relationshipId)
        : base(ExternalEventType.MessageReceived, owner, payload, relationshipId.Value)
    {
    }

    public record EventPayload
    {
        public required string Id { get; init; }
    }
}

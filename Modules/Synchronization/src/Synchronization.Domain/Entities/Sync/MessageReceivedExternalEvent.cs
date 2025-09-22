using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities.Relationships;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class MessageReceivedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    protected MessageReceivedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
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

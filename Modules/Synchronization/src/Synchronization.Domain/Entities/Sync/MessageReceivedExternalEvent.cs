using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class MessageReceivedExternalEvent : ExternalEvent
{
    public MessageReceivedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.MessageReceived, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string Id { get; init; }
    };
}

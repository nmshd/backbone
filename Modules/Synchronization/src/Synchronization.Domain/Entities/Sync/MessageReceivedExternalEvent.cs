using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class MessageReceivedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    private MessageReceivedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
    }

    public MessageReceivedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.MessageReceived, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string Id { get; init; }
    };
}

using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class TokenLockedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    private TokenLockedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
    }

    public TokenLockedExternalEvent(IdentityAddress owner, EventPayload payload) : base(ExternalEventType.TokenLocked, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string TokenId { get; init; }
    }
}

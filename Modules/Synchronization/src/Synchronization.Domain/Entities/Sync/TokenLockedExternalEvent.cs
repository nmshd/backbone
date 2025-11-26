using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class TokenLockedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected TokenLockedExternalEvent()
    {
    }

    public TokenLockedExternalEvent(IdentityAddress owner, EventPayload payload) : base(ExternalEventType.TokenLocked, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string TokenId { get; init; }
    }
}

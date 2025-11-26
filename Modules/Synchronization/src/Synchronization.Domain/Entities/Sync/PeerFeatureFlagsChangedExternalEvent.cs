using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class PeerFeatureFlagsChangedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected PeerFeatureFlagsChangedExternalEvent()
    {
    }

    public PeerFeatureFlagsChangedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.PeerFeatureFlagsChanged, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string PeerAddress { get; init; }
    }
}

using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class PeerFeatureFlagsChangedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    private PeerFeatureFlagsChangedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
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

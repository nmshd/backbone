using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerFeatureFlagsChangedEvent;

public class PeerFeatureFlagsChangedDomainEvent : DomainEvent
{
    public string PeerAddress { get; set; } = null!;
    public string NotifiedIdentityAddress { get; set; } = null!;
}

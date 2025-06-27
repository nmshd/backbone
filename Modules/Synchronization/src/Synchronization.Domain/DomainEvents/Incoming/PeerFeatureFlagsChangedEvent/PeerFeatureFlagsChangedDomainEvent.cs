using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerFeatureFlagsChangedEvent;

public class PeerFeatureFlagsChangedDomainEvent : DomainEvent
{
    public string PeerAddress { get; set; }
    public string NotifiedIdentityAddress { get; set; }
}

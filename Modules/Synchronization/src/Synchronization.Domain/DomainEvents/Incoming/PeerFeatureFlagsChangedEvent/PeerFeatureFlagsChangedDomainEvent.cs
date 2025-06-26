using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerFeatureFlagsChangedEvent;

public class PeerFeatureFlagsChangedDomainEvent : DomainEvent
{
    public PeerFeatureFlagsChangedDomainEvent(string peerAddress, string notifiedIdentityAddress)
    {
        PeerAddress = peerAddress;
        NotifiedIdentityAddress = notifiedIdentityAddress;
    }

    public string PeerAddress { get; set; }
    public string NotifiedIdentityAddress { get; set; }
}

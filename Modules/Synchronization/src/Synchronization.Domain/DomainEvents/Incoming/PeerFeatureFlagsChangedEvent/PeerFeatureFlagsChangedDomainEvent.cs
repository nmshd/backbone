using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerFeatureFlagsChangedEvent;

public class PeerFeatureFlagsChangedDomainEvent : DomainEvent
{
    public required string PeerAddress { get; set; }
    public required string NotifiedIdentityAddress { get; set; }
}

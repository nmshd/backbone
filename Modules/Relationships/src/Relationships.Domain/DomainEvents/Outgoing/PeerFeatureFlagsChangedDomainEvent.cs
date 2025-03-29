using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class PeerFeatureFlagsChangedDomainEvent : DomainEvent
{
    public PeerFeatureFlagsChangedDomainEvent()
    {
    }

    public required string PeerAddress { get; set; }
    public required string NotifiedIdentityAddress { get; set; }
}

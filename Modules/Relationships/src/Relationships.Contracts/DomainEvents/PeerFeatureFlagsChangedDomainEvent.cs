using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Contracts.DomainEvents;

public class PeerFeatureFlagsChangedDomainEvent : DomainEvent
{
    public required string PeerAddress { get; set; }
    public required string NotifiedIdentityAddress { get; set; }
}

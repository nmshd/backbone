using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipStatusChanged;

public class RelationshipStatusChangedDomainEvent : DomainEvent
{
    public required string Initiator { get; set; }
    public required string Peer { get; set; }
}

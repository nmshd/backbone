using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipStatusChanged;

public class RelationshipStatusChangedDomainEvent : DomainEvent
{
    public required string RelationshipId { get; set; }
    public required string Peer { get; set; }
}

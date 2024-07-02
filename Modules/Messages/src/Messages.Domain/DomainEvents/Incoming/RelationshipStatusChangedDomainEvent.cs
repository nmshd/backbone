using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Messages.Domain.DomainEvents.Incoming;

public class RelationshipStatusChangedDomainEvent : DomainEvent
{
    public required string RelationshipId { get; set; }
    public required string Status { get; set; }
    public required string Initiator { get; set; }
    public required string Peer { get; set; }
}

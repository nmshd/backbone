using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipReactivationCompleted;
public class RelationshipReactivationCompletedDomainEvent : DomainEvent
{
    public required string Initiator { get; set; }
    public required string Peer { get; set; }
}

using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Contracts.DomainEvents;

public class RelationshipStatusChangedDomainEvent : DomainEvent
{
    public required string RelationshipId { get; set; }
    public required string NewStatus { get; set; }
    public required string Initiator { get; set; }
    public required string Peer { get; set; }
    public required bool WasDueToIdentityDeletion { get; set; }
}

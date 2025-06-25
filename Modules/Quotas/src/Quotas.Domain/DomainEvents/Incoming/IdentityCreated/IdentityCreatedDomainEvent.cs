using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.IdentityCreated;

public class IdentityCreatedDomainEvent : DomainEvent
{
    public required string Address { get; set; }
    public required string Tier { get; set; }
}

using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TokenCreated;

public class TokenCreatedDomainEvent : DomainEvent
{
    public required string CreatedBy { get; set; }
}

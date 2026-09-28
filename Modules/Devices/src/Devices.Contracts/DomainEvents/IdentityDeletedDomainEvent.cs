using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Contracts.DomainEvents;

public class IdentityDeletedDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; set; }
}

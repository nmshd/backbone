using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Contracts.DomainEvents;

public class IdentityDeletionCancelledDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; set; }
}

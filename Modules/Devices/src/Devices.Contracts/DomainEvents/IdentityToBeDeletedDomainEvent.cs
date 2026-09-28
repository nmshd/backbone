using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Contracts.DomainEvents;

public class IdentityToBeDeletedDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; set; }
    public required DateTime GracePeriodEndsAt { get; set; }
}

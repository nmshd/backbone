using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Contracts.DomainEvents;

public class BackupDeviceUsedDomainEvent : DomainEvent
{
    public required string IdentityAddress { get; set; }
}

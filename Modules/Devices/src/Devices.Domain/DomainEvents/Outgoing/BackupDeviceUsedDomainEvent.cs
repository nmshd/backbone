using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class BackupDeviceUsedDomainEvent : DomainEvent
{
    [Obsolete("For deserialization only", true)]
    public BackupDeviceUsedDomainEvent()
    {
        IdentityAddress = null!;
    }

    public BackupDeviceUsedDomainEvent(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}

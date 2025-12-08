using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class IdentityDeletedDomainEvent : DomainEvent
{
    public IdentityDeletedDomainEvent(string identityAddress) : base($"{identityAddress}/IdentityDeleted")
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; }
}

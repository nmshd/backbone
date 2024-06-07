using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class IdentityDeletedDomainEvent : DomainEvent
{
    public IdentityDeletedDomainEvent(IdentityAddress identityAddress) : base($"{identityAddress}/IdentityDeleted")
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; }
}

using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
public class IdentityToBeDeletedDomainEvent : DomainEvent
{
    public IdentityToBeDeletedDomainEvent(string identityAddress) : base($"{identityAddress}/IdentityToBeDeleted", randomizeId: true)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; }
}

using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
public class IdentityDeletionCanceledDomainEvent : DomainEvent
{
    public IdentityDeletionCanceledDomainEvent(string identityAddress) : base($"{identityAddress}/IdentityDeletionCanceled", randomizeId: true)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; }
}

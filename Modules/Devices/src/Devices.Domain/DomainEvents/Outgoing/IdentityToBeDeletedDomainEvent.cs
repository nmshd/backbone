using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class IdentityToBeDeletedDomainEvent : DomainEvent
{
    public IdentityToBeDeletedDomainEvent(string identityAddress, DateTime gracePeriodEndsAt) : base($"{identityAddress}/IdentityToBeDeleted", randomizeId: true)
    {
        IdentityAddress = identityAddress;
        GracePeriodEndsAt = gracePeriodEndsAt;
    }

    public string IdentityAddress { get; }
    public DateTime GracePeriodEndsAt { get; }
}

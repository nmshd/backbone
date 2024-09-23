using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class IdentityToBeDeletedDomainEvent : DomainEvent
{
    public IdentityToBeDeletedDomainEvent(IdentityAddress identityAddress, DateTime gracePeriodEndsAt) : base($"{identityAddress}/IdentityToBeDeleted", randomizeId: true)
    {
        IdentityAddress = identityAddress;
        GracePeriodEndsAt = gracePeriodEndsAt;
    }

    public string IdentityAddress { get; }
    public DateTime GracePeriodEndsAt { get; }
}

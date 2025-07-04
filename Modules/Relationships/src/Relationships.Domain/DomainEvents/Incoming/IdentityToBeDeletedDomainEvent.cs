using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;

public class IdentityToBeDeletedDomainEvent : DomainEvent
{
    public IdentityToBeDeletedDomainEvent(string identityAddress, DateTime gracePeriodEndsAt)
    {
        IdentityAddress = identityAddress;
        GracePeriodEndsAt = gracePeriodEndsAt;
    }

    public string IdentityAddress { get; }
    public DateTime GracePeriodEndsAt { get; }
}

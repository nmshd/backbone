using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class FeatureFlagsOfIdentityChangedDomainEvent : DomainEvent
{
    public FeatureFlagsOfIdentityChangedDomainEvent(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}

using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class FeatureFlagsOfIdentityChangedDomainEvent : DomainEvent
{
    public FeatureFlagsOfIdentityChangedDomainEvent(Identity identity)
    {
        IdentityAddress = identity.Address.Value;
    }

    public string IdentityAddress { get; set; }
}

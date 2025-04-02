using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class FeatureFlagsOfIdentityChangedDomainEvent : DomainEvent
{
    public FeatureFlagsOfIdentityChangedDomainEvent(Identity identity)
    {
        IdentityAddress = identity.Address;
    }

    public IdentityAddress IdentityAddress { get; set; }
}

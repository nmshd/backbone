using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class FeatureFlagsOfPeerChangedDomainEvent : DomainEvent
{
    public FeatureFlagsOfPeerChangedDomainEvent(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress.Value;
    }

    public string IdentityAddress { get; set; }
}

using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
public class PeerIdentityToBeDeletedDomainEvent : DomainEvent
{
    public PeerIdentityToBeDeletedDomainEvent(IdentityAddress identityAddress) : base($"PeerIdentityToBeDeleted/{identityAddress}")
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; }
}

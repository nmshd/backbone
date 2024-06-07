using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
public class IdentityToBeDeletedDomainEvent : DomainEvent
{
    public IdentityToBeDeletedDomainEvent(IdentityAddress identityAddress) : base($"IdentityToBeDeleted/{identityAddress}", randomizeId: true)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; }
}

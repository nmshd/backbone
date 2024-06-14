using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
public class IdentityDeletionCanceledDomainEvent : DomainEvent
{
    public IdentityDeletionCanceledDomainEvent(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; }
}

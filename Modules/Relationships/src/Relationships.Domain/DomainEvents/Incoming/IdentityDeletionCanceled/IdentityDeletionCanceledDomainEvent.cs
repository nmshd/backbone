using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Incoming.IdentityDeletionCanceled;
public class IdentityDeletionCanceledDomainEvent : DomainEvent
{
    public IdentityDeletionCanceledDomainEvent(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; }
}

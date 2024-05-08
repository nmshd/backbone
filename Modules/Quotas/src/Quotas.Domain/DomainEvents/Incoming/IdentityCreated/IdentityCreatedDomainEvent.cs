using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.IdentityCreated;

public class IdentityCreatedDomainEvent : DomainEvent
{
    public IdentityCreatedDomainEvent(string address, string tier)
    {
        Address = address;
        Tier = tier;
    }

    public string Address { get; private set; }

    public string Tier { get; private set; }
}

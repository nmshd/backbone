using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierCreated;

public class TierCreatedDomainEvent : DomainEvent
{
    public TierCreatedDomainEvent(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; private set; }
    public string Name { get; private set; }
}

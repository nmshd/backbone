using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Domain;

public abstract class Entity
{
    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;


    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

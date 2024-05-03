using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.BuildingBlocks.Domain;

public abstract class Entity
{
    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

    public abstract StronglyTypedId Id { get; }

    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id.Equals(other.Id);
    }

    public static bool operator ==(Entity? a, Entity? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity? a, Entity? b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

public abstract class Entity<TId> : Entity where TId : StronglyTypedId
{
    protected Entity()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
    }

    protected Entity(TId id)
    {
        Id = id;
    }

    public override TId Id { get; }
}

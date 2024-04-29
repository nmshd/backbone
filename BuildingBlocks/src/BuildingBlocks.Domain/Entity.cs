using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Domain;

public abstract class Entity
{
    private readonly List<DomainEvent> _domainEvents = [];

    /*protected Entity()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = default!;
    }

    protected Entity(TId id)
    {
        Id = id;
    }*/

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

    public abstract object Id { get; }

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

        if (GetRealType() != other.GetRealType())
            return false;

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
        return (GetRealType() + Id.ToString()).GetHashCode();
    }

    private Type GetRealType()
    {
        var type = GetType();

        if (type.ToString().Contains("Castle.Proxies"))
            return type.BaseType ?? type;

        return type;
    }
}

public abstract class Entity<TId> : Entity where TId : class, IComparable<TId>
{
    protected Entity()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = default!;
    }

    protected Entity(TId id)
    {
        Id = id;
    }

    public override TId Id { get; }
}

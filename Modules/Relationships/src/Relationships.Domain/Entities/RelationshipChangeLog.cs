using System.Collections;
using Relationships.Domain.Errors;
using Relationships.Domain.Ids;

namespace Relationships.Domain.Entities;

public interface IRelationshipChangeLog : IEnumerable<RelationshipChange>
{
    RelationshipChange GetLatestOfType(RelationshipChangeType type);
    RelationshipChange? GetLatestOfTypeOrNull(RelationshipChangeType type, Predicate<RelationshipChange>? condition = null);
}

public class RelationshipChangeLog : IRelationshipChangeLog, ICollection<RelationshipChange>
{
    private readonly SortedList<DateTime, RelationshipChange> _changes = new();


    public RelationshipChange GetLatestOfType(RelationshipChangeType type)
    {
        var change = _changes.Values.LastOrDefault(c => c.Type == type);

        if (change == null)
            throw new DomainException(DomainErrors.NotFound(nameof(RelationshipChange)));

        return change;
    }

    public RelationshipChange? GetLatestOfTypeOrNull(RelationshipChangeType type, Predicate<RelationshipChange>? condition = null)
    {
        var change = _changes.Values.LastOrDefault(c => c.Type == type && (condition == null || condition(c)));
        return change;
    }


    public RelationshipChange GetById(RelationshipChangeId id)
    {
        var change = _changes.Values.FirstOrDefault(c => c.Id == id);

        if (change == null)
            throw new DomainException(DomainErrors.NotFound(nameof(RelationshipChange)));

        return change;
    }

    #region IEnumerable implementation

    public IEnumerator<RelationshipChange> GetEnumerator()
    {
        return _changes.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region ICollection implementation

    public void Add(RelationshipChange change)
    {
        _changes.Add(change.CreatedAt, change);
    }

    public void Clear()
    {
        _changes.Clear();
    }

    public bool Contains(RelationshipChange item)
    {
        return _changes.Values.Contains(item);
    }

    public void CopyTo(RelationshipChange[] array, int arrayIndex)
    {
        _changes.Values.CopyTo(array, arrayIndex);
    }

    public bool Remove(RelationshipChange item)
    {
        return _changes.Values.Remove(item);
    }

    public int Count => _changes.Count;
    public bool IsReadOnly => false;

    #endregion
}

using System.Collections;
using MediatR;

namespace Backbone.BuildingBlocks.Application.CQRS.BaseClasses;

public abstract class CollectionCommandBase<TResponse, TItem> : IRequest<TResponse>, ICollection<TItem>
{
    private readonly ICollection<TItem> _items = new List<TItem>();

    #region ICollection

    public IEnumerator<TItem> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public void Add(TItem item)
    {
        _items.Add(item);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool Contains(TItem item)
    {
        return _items.Contains(item);
    }

    public void CopyTo(TItem[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    public bool Remove(TItem item)
    {
        return _items.Remove(item);
    }

    public int Count => _items.Count;
    public bool IsReadOnly => _items.IsReadOnly;

    #endregion
}

using System.Collections;

namespace Backbone.ConsumerApi.Tests.Integration.Models;

public abstract class EnumerableResponseBase<TItem> : ICollection<TItem>
{
    private readonly ICollection<TItem> _items = new List<TItem>();

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
}

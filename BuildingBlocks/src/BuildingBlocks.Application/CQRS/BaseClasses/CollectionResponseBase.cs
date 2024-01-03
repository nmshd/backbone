using System.Collections;

namespace Backbone.BuildingBlocks.Application.CQRS.BaseClasses;

public abstract class CollectionResponseBase<TItem> : IEnumerable<TItem>
{
    private readonly IEnumerable<TItem> _items;

    protected CollectionResponseBase(IEnumerable<TItem> items)
    {
        _items = items;
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

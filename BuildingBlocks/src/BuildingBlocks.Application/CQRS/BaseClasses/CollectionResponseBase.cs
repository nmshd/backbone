using System.Collections;

namespace Backbone.BuildingBlocks.Application.CQRS.BaseClasses;

public abstract class CollectionResponseBase<TItem> : IEnumerable<TItem>
{
    private readonly List<TItem> _items;

    protected CollectionResponseBase(IEnumerable<TItem> items)
    {
        // CAUTION: this call is there to be on the save side. Because if, for example, the incoming `IEnumerable` was created with a `Select`,
        // iterating over it multiple times would re-evaluate the `Select` each time, and therefore create new objects each time, leading
        // to unexpected behavior, when for example updating properties of an item within the IEnumerable.
        _items = items.ToList();
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

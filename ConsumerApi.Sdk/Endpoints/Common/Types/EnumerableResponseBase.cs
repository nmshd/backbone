using System.Collections;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

public abstract class EnumerableResponseBase<TItem> : IEnumerable<TItem>
{
    private readonly IEnumerable<TItem> _items;

    protected EnumerableResponseBase(IEnumerable<TItem> items)
    {
        _items = items;
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }
}

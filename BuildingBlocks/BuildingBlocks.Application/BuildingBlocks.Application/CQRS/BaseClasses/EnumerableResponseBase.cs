using System.Collections;

namespace Enmeshed.BuildingBlocks.Application.CQRS.BaseClasses
{
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
            return GetEnumerator();
        }
    }
}
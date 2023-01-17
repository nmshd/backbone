using System.Collections;
using MediatR;

namespace Enmeshed.BuildingBlocks.Application.CQRS.BaseClasses
{
    public abstract class CollectionCommandBase<TResponse, TItem> : IRequest<TResponse>, ICollection<TItem>
    {
        private readonly ICollection<TItem> _events;

        protected CollectionCommandBase()
        {
            _events = new List<TItem>();
        }

        #region ICollection

        public IEnumerator<TItem> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        public void Add(TItem item)
        {
            _events.Add(item);
        }

        public void Clear()
        {
            _events.Clear();
        }

        public bool Contains(TItem item)
        {
            return _events.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _events.CopyTo(array, arrayIndex);
        }

        public bool Remove(TItem item)
        {
            return _events.Remove(item);
        }

        public int Count => _events.Count;
        public bool IsReadOnly => _events.IsReadOnly;

        #endregion
    }
}
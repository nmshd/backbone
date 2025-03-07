using System.Collections.Concurrent;

namespace Backbone.Tooling;

public abstract class ObjectPool<T>
{
    protected ConcurrentBag<T> Objects { get; } = [];

    public async Task<T> Get()
    {
        return Objects.TryTake(out var item) ? item : await CreateObject();
    }

    public void Return(T item)
    {
        Objects.Add(item);
    }

    protected abstract Task<T> CreateObject();
}

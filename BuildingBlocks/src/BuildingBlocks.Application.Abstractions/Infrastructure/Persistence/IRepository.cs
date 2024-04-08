namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence;

public interface IRepository<T, in TId> where T : class
{
    void Add(T entity);

    void AddRange(IEnumerable<T> entities);

    void Remove(TId id);

    void Remove(T entity);

    void RemoveRange(IEnumerable<T> entities);

    void Update(T entity);


    Task<T> Find(TId id);
}

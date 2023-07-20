﻿using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.Repository;

public abstract class AbstractEntityFrameworkRepository<T, TId> : IRepository<T, TId> where T : class
{
    protected readonly DbSet<T> _dbSet;

    protected AbstractEntityFrameworkRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
    }

    public async Task<T> Find(TId id)
    {
        return await _dbSet.FindAsync(id);
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities) Add(entity);
    }

    public async void Remove(TId id)
    {
        var entity = await Find(id);
        Remove(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities) Remove(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
}

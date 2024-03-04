using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Synchronization.Application.Tests;

public static class ApplicationDbContextExtensions
{
    public static TEntity[] SaveEntities<TEntity>(this SynchronizationDbContext dbContext, params TEntity[] entities) where TEntity : class
    {
        dbContext.Set<TEntity>().AddRange(entities);
        dbContext.SaveChanges();
        return entities;
    }

    public static TEntity SaveEntity<TEntity>(this SynchronizationDbContext dbContext, TEntity entity) where TEntity : class
    {
        dbContext.Set<TEntity>().Add(entity);
        dbContext.SaveChanges();
        return entity;
    }
}

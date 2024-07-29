using Microsoft.EntityFrameworkCore;

namespace Backbone.UnitTestTools.Extensions;

public static class ApplicationDbContextExtensions
{
    public static async Task<TEntity[]> SaveEntities<TEntity>(this DbContext dbContext, params TEntity[] entities) where TEntity : class
    {
        dbContext.Set<TEntity>().AddRange(entities);
        await dbContext.SaveChangesAsync();
        return entities;
    }

    public static async Task<TEntity> SaveEntity<TEntity>(this DbContext dbContext, TEntity entity) where TEntity : class
    {
        dbContext.Set<TEntity>().Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public static async Task RemoveEntity<TEntity>(this DbContext dbContext, TEntity entity) where TEntity : class
    {
        dbContext.Set<TEntity>().Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}

using Backbone.Modules.Devices.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Devices.Application.Tests;

public static class ApplicationDbContextExtensions
{
    public static TEntity[] SaveEntities<TEntity>(this DevicesDbContext dbContext, params TEntity[] entities) where TEntity : class
    {
        dbContext.Set<TEntity>().AddRange(entities);
        dbContext.SaveChanges();
        return entities;
    }

    public static TEntity SaveEntity<TEntity>(this DevicesDbContext dbContext, TEntity entity) where TEntity : class
    {
        dbContext.Set<TEntity>().Add(entity);
        dbContext.SaveChanges();
        return entity;
    }
}

using Microsoft.EntityFrameworkCore;

namespace Backbone.UnitTestTools.Extensions;

public static class ApplicationDbContextExtensions
{
    extension(DbContext dbContext)
    {
        public async Task<TEntity[]> SaveEntities<TEntity>(params TEntity[] entities) where TEntity : class
        {
            dbContext.Set<TEntity>().AddRange(entities);
            await dbContext.SaveChangesAsync();
            return entities;
        }

        public async Task<TEntity> SaveEntity<TEntity>(TEntity entity) where TEntity : class
        {
            dbContext.Set<TEntity>().Add(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task RemoveEntity<TEntity>(TEntity entity) where TEntity : class
        {
            dbContext.Set<TEntity>().Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}

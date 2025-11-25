using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Synchronization.Application.Tests;

public static class ApplicationDbContextExtensions
{
    extension(SynchronizationDbContext dbContext)
    {
        public TEntity[] SaveEntities<TEntity>(TEntity entity1, TEntity entity2) where TEntity : class
        {
            dbContext.Set<TEntity>().Add(entity1);
            dbContext.Set<TEntity>().Add(entity2);
            dbContext.SaveChanges();
            return [entity1, entity2];
        }

        public TEntity SaveEntity<TEntity>(TEntity entity) where TEntity : class
        {
            dbContext.Set<TEntity>().Add(entity);
            dbContext.SaveChanges();
            return entity;
        }
    }
}

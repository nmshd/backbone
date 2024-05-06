using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.UnitTestTools.TestDoubles.Fakes;

namespace Backbone.UnitTestTools.BaseClasses;

public abstract class RequestHandlerTestsBase<TDbContext> : AbstractTestsBase where TDbContext : AbstractDbContextBase
{
    protected readonly TDbContext _actContext;
    protected readonly TDbContext _arrangeContext;
    protected readonly TDbContext _assertionContext;

    protected RequestHandlerTestsBase()
    {
        (_arrangeContext, _assertionContext, _actContext) = FakeDbContextFactory.CreateDbContexts<TDbContext>();
    }

    protected TEntity AddToDatabase<TEntity>(TEntity entity) where TEntity : class
    {
        _arrangeContext.Set<TEntity>().Add(entity);
        _arrangeContext.SaveChanges();
        return entity;
    }
}

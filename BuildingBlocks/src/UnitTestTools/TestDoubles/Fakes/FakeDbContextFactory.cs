using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using FakeItEasy;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Backbone.UnitTestTools.TestDoubles.Fakes;

public static class FakeDbContextFactory
{
    public static (TContext arrangeContext, TContext actContext, TContext assertionContext)
        CreateDbContexts<TContext>(SqliteConnection? connection = null) where TContext : AbstractDbContextBase
    {
        return CreateDbContextsInternal<TContext>(connection, A.Dummy<IEventBus>());
    }

    public static (TContext arrangeContext, TContext actContext, TContext assertionContext)
        CreateDbContexts2<TContext>(SqliteConnection? connection = null) where TContext : DbContext
    {
        return CreateDbContextsInternal<TContext>(connection);
    }

    private static (TContext arrangeContext, TContext actContext, TContext assertionContext)
        CreateDbContextsInternal<TContext>(SqliteConnection? connection, params object[] additionalArguments) where TContext : DbContext
    {
        connection ??= CreateDbConnection();
        connection.Open();

        var options = new DbContextOptionsBuilder<TContext>()
            .UseSqlite(connection)
            .Options;

        object[] args = [options, ..additionalArguments];

        var context = (TContext)Activator.CreateInstance(typeof(TContext), args)!;
        context.Database.EnsureCreated();
        context.Dispose();

        var arrangeContext = (TContext)Activator.CreateInstance(typeof(TContext), args)!;
        var assertionContext = (TContext)Activator.CreateInstance(typeof(TContext), args)!;
        var actContext = (TContext)Activator.CreateInstance(typeof(TContext), args)!;

        return (arrangeContext, assertionContext, actContext);
    }

    public static SqliteConnection CreateDbConnection()
    {
        return new SqliteConnection("DataSource=:memory:");
    }
}

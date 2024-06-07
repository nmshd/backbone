using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using FakeItEasy;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Backbone.UnitTestTools.TestDoubles.Fakes;

public static class FakeDbContextFactory
{
    public static (TContext arrangeContext, TContext actContext, TContext assertionContext)
        CreateDbContexts<TContext>(SqliteConnection? connection = null) where TContext : DbContext
    {
        connection ??= CreateDbConnection();
        connection.Open();

        var options = new DbContextOptionsBuilder<TContext>()
            .UseSqlite(connection)
            .Options;

        object[] args = [options, A.Dummy<IEventBus>()];

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

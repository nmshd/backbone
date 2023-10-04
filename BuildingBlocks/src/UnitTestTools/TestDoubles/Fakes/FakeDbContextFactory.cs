using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Enmeshed.UnitTestTools.TestDoubles.Fakes;

public static class FakeDbContextFactory
{
    public static (TContext arrangeContext, TContext assertionContext, TContext actContext, SqliteConnection connection)
        CreateDbContexts<TContext>(SqliteConnection? connection = null) where TContext : DbContext
    {
        connection ??= new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TContext>()
            .UseSqlite(connection)
            .Options;

        object[] args = { options };

        var context = (TContext)Activator.CreateInstance(typeof(TContext), args);
        context.Database.EnsureCreated();
        context.Dispose();

        var arrangeContext = (TContext)Activator.CreateInstance(typeof(TContext), args);
        var assertionContext = (TContext)Activator.CreateInstance(typeof(TContext), args);
        var actContext = (TContext)Activator.CreateInstance(typeof(TContext), args);

        return (arrangeContext, assertionContext, actContext, connection);
    }
}

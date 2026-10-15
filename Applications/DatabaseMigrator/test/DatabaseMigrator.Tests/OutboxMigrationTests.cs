using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.DatabaseMigrator.Tests;

public class OutboxMigrationTests : AbstractTestsBase
{
    [Theory]
    [InlineData("Postgres")]
    [InlineData("SqlServer")]
    public void Both_provider_models_and_idempotent_migration_scripts_include_the_outbox(string provider)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IEventBus>(_ => throw new InvalidOperationException("Migration DbContexts must not resolve the broker."));
        services.AddAllDbContexts(new DatabaseConfiguration
        {
            Provider = provider,
            ConnectionString = provider == "Postgres" ? "Host=localhost;Database=design;Username=design" : "Server=localhost;Database=design;Integrated Security=true"
        });
        using var container = services.BuildServiceProvider();
        using var scope = container.CreateScope();
        foreach (var database in container.GetServices<OutboxDatabase>())
        {
            var context = (DbContext)scope.ServiceProvider.GetRequiredService(database.ContextType);
            context.Database.HasPendingModelChanges().ShouldBeFalse();
            var script = context.GetService<IMigrator>().GenerateScript(options: MigrationsSqlGenerationOptions.Idempotent);
            script.ShouldContain("OutboxMessages");
            script.ShouldContain("InboxMessages");
            script.ShouldContain("IX_OutboxMessages_SaveId_Stream_Sequence");
        }
    }

    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task All_module_migrations_apply_and_match_the_runtime_models(string provider)
    {
        var connection = Environment.GetEnvironmentVariable("BACKBONE_OUTBOX_TEST_" + provider);
        Assert.SkipWhen(string.IsNullOrWhiteSpace(connection), "No dedicated " + provider + " test database connection configured.");
        var name = "backbone_outbox_migrations_" + Guid.NewGuid().ToString("N");
        connection = provider == "POSTGRES"
            ? new Npgsql.NpgsqlConnectionStringBuilder(connection) { Database = name }.ConnectionString
            : new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connection) { InitialCatalog = name }.ConnectionString;
        var services = new ServiceCollection();
        services.AddSingleton<IEventBus>(_ => throw new InvalidOperationException("Migration DbContexts must not resolve the broker."));
        services.AddAllDbContexts(new DatabaseConfiguration { Provider = provider == "POSTGRES" ? "Postgres" : "SqlServer", ConnectionString = connection! });
        await using var container = services.BuildServiceProvider();
        await using var scope = container.CreateAsyncScope();
        var databases = container.GetServices<OutboxDatabase>().ToList();
        var first = (DbContext)scope.ServiceProvider.GetRequiredService(databases[0].ContextType);
        await first.Database.GetService<IRelationalDatabaseCreator>().CreateAsync(TestContext.Current.CancellationToken);
        try
        {
            var contexts = new DbContextProvider(container);
            var migrations = await new MigrationReader(contexts).ReadMigrations();
            foreach (var migration in migrations)
            {
                await using var context = contexts.GetDbContext(migration.DbContextType);
                await context.GetService<IMigrator>().MigrateAsync(migration.Name, TestContext.Current.CancellationToken);
            }
            foreach (var database in databases)
            {
                var context = (DbContext)scope.ServiceProvider.GetRequiredService(database.ContextType);
                context.Database.HasPendingModelChanges().ShouldBeFalse();
                (await context.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
                (await context.Set<InboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
            }
        }
        finally
        {
            // Only the database generated above is ever removed.
            await first.Database.EnsureDeletedAsync(CancellationToken.None);
        }
    }
}

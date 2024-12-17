using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;

namespace Backbone.BuildingBlocks.API.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDbContext<TContext>(this IHost host) where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Migrating database associated with context '{context}'", typeof(TContext).Name);

            var retry = Policy.Handle<SqlException>().Or<PostgresException>()
                .WaitAndRetry([
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(15)
                ]);

            retry.Execute(context.Database.Migrate);

            logger.LogInformation("Migrated database associated with context '{context}'", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An error occurred while migrating the database associated with context {context}", typeof(TContext).Name);
            throw;
        }

        return host;
    }

    public static IHost SeedDbContext<TContext, TSeeder>(this IHost host) where TContext : DbContext where TSeeder : IDbSeeder<TContext>
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();
        var seeder = services.GetRequiredService<TSeeder>();

        try
        {
            logger.LogInformation("Seeding database associated with context '{context}'", typeof(TContext).Name);

            seeder.SeedAsync(context).GetAwaiter().GetResult();

            logger.LogInformation("Seeded database associated with context '{context}'", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An error occurred while seeding the database associated with context {context}", typeof(TContext).Name);
            throw;
        }

        return host;
    }
}

public interface IDbSeeder<in T> where T : DbContext
{
    Task SeedAsync(T context);
}

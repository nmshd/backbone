using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace Backbone.BuildingBlocks.API.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDbContext<TContext>(this IHost host,
        Action<TContext, IServiceProvider>? seeder = null) where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILogger<TContext>>();

        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Migrating database associated with context '{context}'", typeof(TContext).Name);

            var retry = Policy.Handle<SqlException>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(15)
                });

            retry.Execute(context.Database.Migrate);

            seeder?.Invoke(context, services);

            logger.LogInformation("Migrated database associated with context '{context}'", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An error occurred while migrating the database used on context {context}", typeof(TContext).Name);
            throw;
        }

        return host;
    }
}

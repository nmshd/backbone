using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, Action<PersistenceOptions> setupOptions)
    {
        var options = new PersistenceOptions();
        setupOptions.Invoke(options);

        services.AddPersistence(options);
    }

    public static void AddPersistence(this IServiceCollection services, PersistenceOptions options)
    {
        services.AddDatabase(options.DbOptions);
    }
}

public class PersistenceOptions
{
    public DatabaseConfiguration DbOptions { get; set; } = new();
}

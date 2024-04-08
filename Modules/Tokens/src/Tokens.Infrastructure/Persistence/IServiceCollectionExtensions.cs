using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence;

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
        services.AddRepositories();
    }
}

public class PersistenceOptions
{
    public DbOptions DbOptions { get; set; } = new();
}

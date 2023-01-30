using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Messages.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, Action<PersistenceOptions> setupOptions)
    {
        var options = new PersistenceOptions();
        setupOptions?.Invoke(options);

        services.AddPersistence(options);
    }

    public static void AddPersistence(this IServiceCollection services, PersistenceOptions options)
    {
        services.AddDatabase(options.DbOptions);
        services.AddBlobStorage(options.BlobStorageOptions);
    }
}

public class PersistenceOptions
{
    public DbOptions DbOptions { get; set; } = new();
    public BlobStorageOptions BlobStorageOptions { get; set; } = new();
}

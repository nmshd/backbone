using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Backbone.Synchronization.Application.Infrastructure;
using Backbone.Synchronization.Infrastructure.Persistence.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Synchronization.Infrastructure.Persistence;

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
        services.Configure<BlobOptions>(blobOptions =>
            blobOptions.RootFolder = options.BlobStorageOptions.Container);
        services.AddBlobStorage(options.BlobStorageOptions);
    }
}

public class PersistenceOptions
{
    public DbOptions DbOptions { get; set; } = new();
    public BlobStorageOptions BlobStorageOptions { get; set; } = new();
}

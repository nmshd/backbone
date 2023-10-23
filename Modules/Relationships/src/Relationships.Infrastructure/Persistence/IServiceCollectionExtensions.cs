using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Backbone.Relationships.Application.Infrastructure;
using Backbone.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Relationships.Infrastructure.Persistence.Database;
using Backbone.Relationships.Infrastructure.Persistence.Database.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Relationships.Infrastructure.Persistence;

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

        services.AddTransient<IRelationshipsRepository, RelationshipsRepository>();
        services.AddTransient<IRelationshipTemplatesRepository, RelationshipTemplatesRepository>();
    }
}

public class PersistenceOptions
{
    public global::Backbone.Relationships.Infrastructure.Persistence.Database.IServiceCollectionExtensions.DbOptions DbOptions { get; set; } = new();
    public BlobStorageOptions BlobStorageOptions { get; set; } = new();
}

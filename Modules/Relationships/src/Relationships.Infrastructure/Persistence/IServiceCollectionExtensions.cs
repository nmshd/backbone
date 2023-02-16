﻿using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Infrastructure.Persistence.ContentStore;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence;

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
        services.AddScoped<IContentStore, BlobStorageContentStore>();
    }
}

public class PersistenceOptions
{
    public global::Backbone.Modules.Relationships.Infrastructure.Persistence.Database.IServiceCollectionExtensions.DbOptions DbOptions { get; set; } = new();
    public BlobStorageOptions BlobStorageOptions { get; set; } = new();
}

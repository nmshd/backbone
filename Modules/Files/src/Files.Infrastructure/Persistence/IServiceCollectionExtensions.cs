using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Files.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, DatabaseConfiguration dbOptions, BlobStorageOptions blobStorageOptions)
    {
        services.AddDatabase(dbOptions);
        services.AddBlobStorage(blobStorageOptions);

        services.Configure<BlobConfiguration>(blobOptions => blobOptions.RootFolder = blobStorageOptions.RootFolder);
        services.AddTransient<IFilesRepository, FilesRepository>();
    }
}

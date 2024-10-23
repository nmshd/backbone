using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Tooling.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;

public static class GoogleCloudStorageServiceCollectionExtensions
{
    public static void AddGoogleCloudStorage(this IServiceCollection services, BlobStorageOptions.GoogleCloudStorageOptions options)
    {
        services.AddSingleton(_ =>
        {
            var storageClient = options.ServiceAccountJson.IsNullOrEmpty()
                ? StorageClient.Create()
                : StorageClient.Create(GoogleCredential.FromJson(options.ServiceAccountJson));
            return storageClient;
        });

        services.AddScoped<IBlobStorage>(sp =>
        {
            var storageClient = sp.GetService<StorageClient>();
            var logger = sp.GetService<ILogger<GoogleCloudStorage>>();

            if (storageClient == null)
            {
                throw new Exception("A StorageClient was not registered in the dependency container.");
            }

            if (logger == null)
            {
                throw new Exception("A Logger was not registered in the dependency container.");
            }

            return new GoogleCloudStorage(storageClient, logger);
        });
    }
}

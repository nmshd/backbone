using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Tooling.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;

public static class GoogleCloudStorageServiceCollectionExtensions
{
    public static void AddGoogleCloudStorage(this IServiceCollection services,
        Action<GoogleCloudStorageOptions> setupOptions)
    {
        services.Configure(setupOptions);

        var options = new GoogleCloudStorageOptions();
        setupOptions.Invoke(options);

        services.AddGoogleCloudStorage(options);
    }

    public static void AddGoogleCloudStorage(this IServiceCollection services, GoogleCloudStorageOptions options)
    {
        services.AddSingleton(_ =>
        {
            var storageClient = options.GcpAuthJson.IsNullOrEmpty()
                ? StorageClient.Create()
                : StorageClient.Create(GoogleCredential.FromJson(options.GcpAuthJson));
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

public class GoogleCloudStorageOptions
{
    public string? GcpAuthJson { get; set; }
    public string BucketName { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Tooling.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;

public static class GoogleCloudStorageServiceCollectionExtensions
{
    public static void AddGoogleCloudStorage(this IServiceCollection services, GoogleCloudStorageConfiguration configuration)
    {
        services.AddSingleton(_ =>
        {
            var storageClient = configuration.ServiceAccountJson.IsNullOrEmpty()
                ? StorageClient.Create()
                : StorageClient.Create(GoogleCredential.FromJson(configuration.ServiceAccountJson));
            return storageClient;
        });

        services.AddScoped<IBlobStorage, GoogleCloudStorage>();
    }
}

public class GoogleCloudStorageConfiguration
{
    public string? ServiceAccountJson { get; set; }

    [Required]
    [MinLength(2)]
    public required string BucketName { get; set; }
}

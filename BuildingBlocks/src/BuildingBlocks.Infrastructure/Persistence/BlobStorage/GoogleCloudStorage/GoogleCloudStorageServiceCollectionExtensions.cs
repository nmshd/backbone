using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Tooling.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;

public static class GoogleCloudStorageServiceCollectionExtensions
{
    public static void AddGoogleCloudStorage(this IServiceCollection services, GoogleCloudStorageOptions options)
    {
        services.AddSingleton(_ =>
        {
            var storageClient = options.ServiceAccountJson.IsNullOrEmpty()
                ? StorageClient.Create()
                : StorageClient.Create(GoogleCredential.FromJson(options.ServiceAccountJson));
            return storageClient;
        });

        services.AddScoped<IBlobStorage, GoogleCloudStorage>();
    }
}

public class GoogleCloudStorageOptions
{
    public required string? ServiceAccountJson { get; set; }

    [Required]
    [MinLength(2)]
    public required string BucketName { get; set; }
}

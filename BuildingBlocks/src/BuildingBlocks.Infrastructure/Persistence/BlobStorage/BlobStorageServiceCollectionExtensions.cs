using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;

public static class BlobStorageServiceCollectionExtensions
{
    public const string AZURE_CLOUD_PROVIDER = "Azure";
    public const string GOOGLE_CLOUD_PROVIDER = "GoogleCloud";

    public static void AddBlobStorage(this IServiceCollection services, Action<BlobStorageOptions> setupOptions)
    {
        var options = new BlobStorageOptions();
        setupOptions.Invoke(options);

        AddBlobStorage(services, options);
    }

    public static void AddBlobStorage(this IServiceCollection services, BlobStorageOptions options)
    {
        switch (options.CloudProvider)
        {
            case AZURE_CLOUD_PROVIDER:
                services.AddAzureStorageAccount(azureStorageAccountOptions => { azureStorageAccountOptions.ConnectionString = options.ConnectionInfo!; });
                break;
            case GOOGLE_CLOUD_PROVIDER:
                services.AddGoogleCloudStorage(googleCloudStorageOptions =>
                {
                    googleCloudStorageOptions.GcpAuthJson = options.ConnectionInfo;
                    googleCloudStorageOptions.BucketName = options.Container;
                });
                break;
            default:
            {
                if (options.CloudProvider.IsNullOrEmpty())
                    throw new NotSupportedException("No cloud provider was specified.");

                throw new NotSupportedException(
                    $"{options.CloudProvider} is not a currently supported cloud provider.");
            }
        }

        services.AddHealthChecks().Add(
            new HealthCheckRegistration(
                "blob_storage",
                sp => new BlobStorageHealthCheck(sp.GetRequiredService<IBlobStorage>(), options.Container),
                HealthStatus.Unhealthy, null
            )
        );
    }
}

public class BlobStorageOptions
{
    public string CloudProvider { get; set; } = null!;

    public string Container { get; set; } = null!;

    public string? ConnectionInfo { get; set; }
}

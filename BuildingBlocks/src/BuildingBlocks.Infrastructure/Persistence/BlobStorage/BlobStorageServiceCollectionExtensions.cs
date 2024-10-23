using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;

public static class BlobStorageServiceCollectionExtensions
{
    public static void AddBlobStorage(this IServiceCollection services, Action<BlobStorageOptions> setupOptions)
    {
        var options = new BlobStorageOptions();
        setupOptions.Invoke(options);

        AddBlobStorage(services, options);
    }

    public static void AddBlobStorage(this IServiceCollection services, BlobStorageOptions options)
    {
        switch (options.ProductName)
        {
            case BlobStorageOptions.AZURE_STORAGE_ACCOUNT:
                services.AddAzureStorageAccount(options.AzureStorageAccount!);
                break;
            case BlobStorageOptions.GOOGLE_CLOUD_STORAGE:
                services.AddGoogleCloudStorage(options.GoogleCloudStorage!);
                break;
            default:
            {
                if (options.ProductName.IsNullOrEmpty())
                    throw new NotSupportedException("No cloud provider was specified.");

                throw new NotSupportedException(
                    $"{options.ProductName} is not a currently supported cloud provider.");
            }
        }

        services.AddHealthChecks().Add(
            new HealthCheckRegistration(
                "blob_storage",
                sp => new BlobStorageHealthCheck(sp.GetRequiredService<IBlobStorage>(), options.RootFolder),
                HealthStatus.Unhealthy, null
            )
        );
    }
}

public class BlobStorageOptions : IValidatableObject
{
    public const string AZURE_STORAGE_ACCOUNT = "AzureStorageAccount";
    public const string GOOGLE_CLOUD_STORAGE = "GoogleCloudStorage";

    [RegularExpression($"{AZURE_STORAGE_ACCOUNT}|{GOOGLE_CLOUD_STORAGE}")]
    public string ProductName { get; set; } = null!;

    public AzureStorageAccountOptions? AzureStorageAccount { get; set; }

    public GoogleCloudStorageOptions? GoogleCloudStorage { get; set; }

    public string RootFolder => ProductName switch
    {
        AZURE_STORAGE_ACCOUNT => AzureStorageAccount!.ContainerName,
        GOOGLE_CLOUD_STORAGE => GoogleCloudStorage!.BucketName,
        _ => throw new Exception("Unsupported ProductName")
    };

    public class AzureStorageAccountOptions
    {
        public required string ConnectionString { get; set; }

        [Required]
        [MinLength(2)]
        public required string ContainerName { get; set; }
    }

    public class GoogleCloudStorageOptions
    {
        public required string? ServiceAccountJson { get; set; }

        [Required]
        [MinLength(2)]
        public required string BucketName { get; set; }
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ProductName == AZURE_STORAGE_ACCOUNT && AzureStorageAccount == null)
            yield return new ValidationResult($"The property '{nameof(AzureStorageAccount)}' must be set when the {nameof(ProductName)} is '{AZURE_STORAGE_ACCOUNT}'.", [nameof(AzureStorageAccount)]);

        if (ProductName == GOOGLE_CLOUD_STORAGE && GoogleCloudStorage == null)
            yield return new ValidationResult($"The property '{nameof(GoogleCloudStorage)}' must be set when the {nameof(ProductName)} is '{GOOGLE_CLOUD_STORAGE}'.", [nameof(GoogleCloudStorage)]);
    }
}

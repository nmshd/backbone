using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.Ionos;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;

public static class BlobStorageServiceCollectionExtensions
{
    public const string AZURE_CLOUD_PROVIDER = "Azure";
    public const string GOOGLE_CLOUD_PROVIDER = "GoogleCloud";
    public const string IONOS_CLOUD_PROVIDER = "Ionos";

    public static void AddBlobStorage(this IServiceCollection services, Action<BlobStorageOptions> setupOptions)
    {
        var options = new BlobStorageOptions();
        setupOptions.Invoke(options);

        AddBlobStorage(services, options);
    }

    public static void AddBlobStorage(this IServiceCollection services, BlobStorageOptions options)
    {
        if (options.CloudProvider == AZURE_CLOUD_PROVIDER)
            services.AddAzureStorageAccount(azureStorageAccountOptions =>
            {
                azureStorageAccountOptions.ConnectionString = options.ConnectionInfo!;
            });
        else if (options.CloudProvider == GOOGLE_CLOUD_PROVIDER)
            services.AddGoogleCloudStorage(googleCloudStorageOptions =>
            {
                googleCloudStorageOptions.GcpAuthJson = options.ConnectionInfo;
                googleCloudStorageOptions.BucketName = options.Container;
            });
        else if (options.CloudProvider == IONOS_CLOUD_PROVIDER)
        {
            services.Configure<IonosS3Options>(opt =>
            {
                opt.ServiceUrl = options.IonosS3Config.ServiceUrl;
                opt.AccessKey = options.IonosS3Config.AccessKey;
                opt.SecretKey = options.IonosS3Config.SecretKey;
                opt.BucketName = options.IonosS3Config.BucketName;
            });

            services.AddSingleton<IonosS3ClientFactory>();
            services.AddScoped<IBlobStorage, IonosS3BlobStorage>();
        }
        else if (options.CloudProvider.IsNullOrEmpty())
            throw new NotSupportedException("No cloud provider was specified.");
        else
            throw new NotSupportedException(
                $"{options.CloudProvider} is not a currently supported cloud provider.");
    }
}

public class BlobStorageOptions
{
    [Required]
    [MinLength(1)]
    [RegularExpression("Azure|GoogleCloud|Ionos")]
    public string CloudProvider { get; set; } = string.Empty;

    public string? ConnectionInfo { get; set; } = string.Empty;
    public string? Container { get; set; } = string.Empty;

    public IonosS3Config? IonosS3Config { get; set; } = new IonosS3Config();
}

public class IonosS3Config
{
    [Required]
    public string ServiceUrl { get; set; } = string.Empty;

    [Required]
    public string AccessKey { get; set; } = string.Empty;

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string BucketName { get; set; } = string.Empty;
}


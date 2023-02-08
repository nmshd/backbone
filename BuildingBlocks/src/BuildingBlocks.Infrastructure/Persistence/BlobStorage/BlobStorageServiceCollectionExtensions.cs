﻿using Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;
using Enmeshed.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage;

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
        if (options.CloudProvider == AZURE_CLOUD_PROVIDER)
            services.AddAzureStorageAccount(azureStorageAccountOptions =>
            {
                azureStorageAccountOptions.ConnectionString = options.ConnectionInfo;
                azureStorageAccountOptions.ContainerName = options.Container;
            });
        else if (options.CloudProvider == GOOGLE_CLOUD_PROVIDER)
            services.AddGoogleCloudStorage(googleCloudStorageOptions =>
            {
                googleCloudStorageOptions.GCPAuthJson = options.ConnectionInfo;
                googleCloudStorageOptions.BucketName = options.Container;
            });
        else if (options.CloudProvider.IsEmpty())
            throw new NotSupportedException("No cloud provider was specified.");
        else
            throw new NotSupportedException(
                $"{options.CloudProvider} is not a currently supported cloud provider.");
    }
}

public class BlobStorageOptions
{
    public string ConnectionInfo { get; set; } = string.Empty;
    public string Container { get; set; } = string.Empty;
    public string CloudProvider { get; set; } = string.Empty;
}
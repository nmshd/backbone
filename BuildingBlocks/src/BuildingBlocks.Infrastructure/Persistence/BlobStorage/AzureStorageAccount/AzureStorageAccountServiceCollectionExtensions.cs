using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class AzureStorageAccountServiceCollectionExtensions
{
    public static void AddAzureStorageAccount(this IServiceCollection services,
        Action<AzureStorageAccountOptions> setupOptions)
    {
        var options = new AzureStorageAccountOptions();
        setupOptions.Invoke(options);

        services.AddAzureStorageAccount(options);
    }


    public static void AddAzureStorageAccount(this IServiceCollection services, AzureStorageAccountOptions options)
    {
        services.AddSingleton(_ =>
        {
            var containerClient = new BlobContainerClient(options.ConnectionString, options.ContainerName);
            containerClient.CreateIfNotExists();

            try
            {
                containerClient.SetAccessPolicy(PublicAccessType.Blob);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return containerClient;
        });

        services.AddScoped<IBlobStorage, AzureStorageAccount>();
    }
}

public class AzureStorageAccountOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}
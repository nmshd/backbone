using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureBlobStorage;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureBlobStorageServiceCollectionExtensions
    {
        public static void AddAzureStorageAccount(this IServiceCollection services, Action<AzureStorageAccountOptions> setupOptions)
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

            services.AddScoped<IBlobStorage, AzureBlobStorage>();
        }
    }

    public class AzureStorageAccountOptions
    {
#pragma warning disable CS8618
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
#pragma warning restore CS8618
    }
}

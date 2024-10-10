using Azure.Storage.Blobs;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using HealthChecks.Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;

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
        services.Configure<AzureStorageAccountOptions>(opt => opt.ConnectionString = options.ConnectionString);
        services.AddSingleton<AzureStorageAccountContainerClientFactory>();
        services.AddScoped<IBlobStorage, AzureStorageAccount>();
        services.AddSingleton(_ => new BlobServiceClient(options.ConnectionString));

        services.AddHealthChecks().AddAzureBlobStorage(
            optionsFactory: sp => new AzureBlobStorageHealthCheckOptions { ContainerName = sp.GetRequiredService<IOptions<BlobStorageOptions>>().Value.Container }
        );
    }
}

public class AzureStorageAccountOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}

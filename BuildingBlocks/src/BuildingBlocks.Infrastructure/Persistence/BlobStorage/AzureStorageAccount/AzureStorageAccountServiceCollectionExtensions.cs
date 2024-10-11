using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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

        services.AddHealthChecks().Add(
            new HealthCheckRegistration(
                "blob_storage",
                sp => new BlobStorageHealthCheck(sp.GetRequiredService<IBlobStorage>(), options.BucketName),
                HealthStatus.Unhealthy, null
            )
        );
    }
}

public class AzureStorageAccountOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
}

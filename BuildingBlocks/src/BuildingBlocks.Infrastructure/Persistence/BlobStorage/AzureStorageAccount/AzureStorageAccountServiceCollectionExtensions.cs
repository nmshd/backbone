using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;

public static class AzureStorageAccountServiceCollectionExtensions
{
    public static void AddAzureStorageAccount(this IServiceCollection services, AzureStorageAccountConfiguration configuration)
    {
        services.AddSingleton<IOptions<AzureStorageAccountConfiguration>>(new OptionsWrapper<AzureStorageAccountConfiguration>(configuration));
        services.Configure<AzureStorageAccountConfiguration>(opt =>
        {
            opt.ConnectionString = configuration.ConnectionString;
            opt.ContainerName = configuration.ContainerName;
        });
        services.AddSingleton<AzureStorageAccountContainerClientFactory>();
        services.AddScoped<IBlobStorage, AzureStorageAccount>();
    }
}

public class AzureStorageAccountConfiguration
{
    [Required]
    [MinLength(2)]
    public required string ConnectionString { get; set; }

    [Required]
    [MinLength(2)]
    public required string ContainerName { get; set; }
}

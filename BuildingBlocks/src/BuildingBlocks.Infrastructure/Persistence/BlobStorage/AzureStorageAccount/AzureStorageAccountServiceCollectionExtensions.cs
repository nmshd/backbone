using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;

public static class AzureStorageAccountServiceCollectionExtensions
{
    public static void AddAzureStorageAccount(this IServiceCollection services, AzureStorageAccountOptions options)
    {
        services.AddSingleton<IOptions<AzureStorageAccountOptions>>(new OptionsWrapper<AzureStorageAccountOptions>(options));
        services.Configure<AzureStorageAccountOptions>(opt =>
        {
            opt.ConnectionString = options.ConnectionString;
            opt.ContainerName = options.ContainerName;
        });
        services.AddSingleton<AzureStorageAccountContainerClientFactory>();
        services.AddScoped<IBlobStorage, AzureStorageAccount>();
    }
}

public class AzureStorageAccountOptions
{
    [Required]
    [MinLength(2)]
    public required string ConnectionString { get; set; }

    [Required]
    [MinLength(2)]
    public required string ContainerName { get; set; }
}

using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public static class AzureServiceBusServiceCollectionExtensions
{
    public static void AddAzureServiceBus(this IServiceCollection services, ServiceBusOptions options)
    {
        services.AddAzureClients(azureBuilder =>
        {
            azureBuilder.AddServiceBusClient(options.ConnectionString);
            azureBuilder.AddServiceBusAdministrationClient(options.ConnectionString);
        });

        services.AddSingleton<IEventBus, EventBusAzureServiceBus>();
    }
}

public class ServiceBusOptions
{
    [Required]
    [MinLength(10)]
    public string ConnectionString { get; set; } = null!;
}

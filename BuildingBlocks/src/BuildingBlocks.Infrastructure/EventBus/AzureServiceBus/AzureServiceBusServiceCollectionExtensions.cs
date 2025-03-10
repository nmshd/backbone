using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public static class AzureServiceBusServiceCollectionExtensions
{
    public static void AddAzureServiceBus(this IServiceCollection services, Action<ServiceBusOptions> setupOptions)
    {
        var options = new ServiceBusOptions();
        setupOptions.Invoke(options);

        services.AddAzureClients(azureBuilder =>
        {
            azureBuilder.AddServiceBusClient(options.ConnectionString);
            azureBuilder.AddServiceBusAdministrationClient(options.ConnectionString);
        });

        services.AddSingleton<IEventBus, EventBusAzureServiceBus>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<EventBusAzureServiceBus>>();
            return new EventBusAzureServiceBus(
                sp.GetRequiredService<ServiceBusClient>(),
                sp.GetRequiredService<ServiceBusAdministrationClient>(),
                logger,
                sp,
                options.HandlerRetryBehavior
            );
        });
    }
}

public class ServiceBusOptions : BasicBusOptions
{
    public string ConnectionString { get; set; } = null!;
}

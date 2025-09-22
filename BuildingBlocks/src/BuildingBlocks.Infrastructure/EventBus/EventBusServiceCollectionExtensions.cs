using System.Diagnostics.Metrics;
using Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public static class EventBusServiceCollectionExtensions
{
    public static void AddEventBus(this IServiceCollection services, EventBusConfiguration configuration, string meterName)
    {
        services.AddSingleton(new Meter(meterName));

        services.AddSingleton<EventBusMetrics>();

        switch (configuration.ProductName)
        {
            case EventBusConfiguration.AZURE:
                services.AddAzureServiceBus(configuration.AzureServiceBus);
                break;
            case EventBusConfiguration.GOOGLE_CLOUD:
                services.AddGoogleCloudPubSub(configuration.GoogleCloudPubSub);
                break;
            case EventBusConfiguration.RABBIT_MQ:
                services.AddRabbitMq(configuration.RabbitMq);
                break;
            case "":
                throw new NotSupportedException("No event bus product name was specified.");
            default:
                throw new NotSupportedException($"{configuration.ProductName} is not a currently supported event bus product name.");
        }
    }
}

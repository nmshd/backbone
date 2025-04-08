using System.Diagnostics.Metrics;
using Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public static class EventBusServiceCollectionExtensions
{
    public const string AZURE = "AzureServiceBus";
    public const string GOOGLE_CLOUD = "GoogleCloudPubSub";
    public const string RABBIT_MQ = "RabbitMQ";

    public static void AddEventBus(this IServiceCollection services, EventBusConfiguration configuration, string meterName)
    {
        services.AddSingleton(new Meter(meterName));

        services.AddSingleton<EventBusMetrics>();

        switch (configuration.ProductName)
        {
            case AZURE:
                services.AddAzureServiceBus(configuration.AzureServiceBus);
                break;
            case GOOGLE_CLOUD:
                services.AddGoogleCloudPubSub(configuration.GoogleCloudPubSub);
                break;
            case RABBIT_MQ:
                services.AddRabbitMq(configuration.RabbitMq);
                break;
            case "":
                throw new NotSupportedException("No event bus product name was specified.");
            default:
                throw new NotSupportedException($"{configuration.ProductName} is not a currently supported event bus product name.");
        }
    }
}

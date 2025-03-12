using Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Infrastructure.EventBus;

public static class EventBusServiceCollectionExtensions
{
    public const string AZURE = "AzureServiceBus";
    public const string GOOGLE_CLOUD = "GoogleCloudPubSub";
    public const string RABBIT_MQ = "RabbitMQ";

    public static void AddEventBus(this IServiceCollection services, EventBusOptions options)
    {
        switch (options.ProductName)
        {
            case AZURE:
                services.AddAzureServiceBus(options.AzureServiceBus);
                break;
            case GOOGLE_CLOUD:
                services.AddGoogleCloudPubSub(options.GoogleCloudPubSub);
                break;
            case RABBIT_MQ:
                services.AddRabbitMq(options.RabbitMq);
                break;
            case "":
                throw new NotSupportedException("No event bus product name was specified.");
            default:
                throw new NotSupportedException($"{options.ProductName} is not a currently supported event bus product name.");
        }
    }
}

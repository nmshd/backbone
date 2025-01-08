using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Infrastructure.EventBus;

public static class EventBusServiceCollectionExtensions
{
    public const string AZURE = "Azure";
    public const string GOOGLE_CLOUD = "GoogleCloud";
    public const string RABBIT_MQ = "RabbitMQ";

    public static void AddEventBus(this IServiceCollection services, EventBusConfiguration configuration)
    {
        switch (configuration.Vendor)
        {
            case AZURE:
                services.AddAzureServiceBus(options =>
                {
                    LoadBasicBusOptions(configuration, options);
                    options.ConnectionString = configuration.ConnectionInfo;
                });
                break;
            case GOOGLE_CLOUD:
                services.AddGoogleCloudPubSub(options =>
                {
                    LoadBasicBusOptions(configuration, options);
                    options.ProjectId = configuration.GcpPubSubProjectId;
                    options.TopicName = configuration.GcpPubSubTopicName;
                    options.ConnectionInfo = configuration.ConnectionInfo;
                });
                break;
            case RABBIT_MQ:
                services.AddRabbitMq(options =>
                {
                    LoadBasicBusOptions(configuration, options);
                    options.HostName = configuration.ConnectionInfo;
                    options.Username = configuration.RabbitMqUsername;
                    options.Password = configuration.RabbitMqPassword;
                    options.ExchangeName = configuration.RabbitMqExchangeName;
                    options.QueueName = configuration.RabbitMqQueueName;
                    options.ConnectionRetryCount = configuration.ConnectionRetryCount;
                });
                break;
            case "":
                throw new NotSupportedException("No event bus vendor was specified.");
            default:
                throw new NotSupportedException(
                    $"{configuration.Vendor} is not a currently supported event bus vendor.");
        }
    }

    private static void LoadBasicBusOptions<T>(EventBusConfiguration configuration, T options) where T : BasicBusOptions
    {
        options.SubscriptionClientName = configuration.SubscriptionClientName;
        options.HandlerRetryBehavior = configuration.HandlerRetryBehavior;
    }
}

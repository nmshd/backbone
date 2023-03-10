using Enmeshed.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
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
                    options.ConnectionString = configuration.ConnectionInfo;
                    options.SubscriptionClientName = configuration.SubscriptionClientName;
                });
                break;
            case GOOGLE_CLOUD:
                services.AddGoogleCloudPubSub(options =>
                {
                    options.ProjectId = configuration.GcpPubSubProjectId;
                    options.TopicName = configuration.GcpPubSubTopicName;
                    options.SubscriptionClientName = configuration.SubscriptionClientName;
                    options.ConnectionInfo = configuration.ConnectionInfo;
                });
                break;
            case RABBIT_MQ:
                services.AddRabbitMQ(options =>
                {
                    options.HostName = configuration.ConnectionInfo;
                    options.Username = configuration.RabbitMQUsername;
                    options.Password = configuration.RabbitMQPassword;
                    options.SubscriptionClientName = configuration.SubscriptionClientName;
                    options.RetryCount = configuration.ConnectionRetryCount;
                });
                break;
            case "":
                throw new NotSupportedException("No event bus vendor was specified.");
            default:
                throw new NotSupportedException(
                    $"{configuration.Vendor} is not a currently supported event bus vendor.");
        }
    }
}
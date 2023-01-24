using Microsoft.Extensions.DependencyInjection;

namespace Synchronization.Infrastructure.EventBus;

public static class EventBusServiceCollectionExtensions
{
    public const string AZURE = "Azure";
    public const string GOOGLE_CLOUD = "GoogleCloud";
    public const string RABBIT_MQ = "RabbitMQ";

    public static void AddEventBus(this IServiceCollection services, EventBusConfiguration configuration)
    {
        if (configuration.Vendor == AZURE)
            services.AddAzureServiceBus(options =>
            {
                options.ConnectionString = configuration.ConnectionInfo;
                options.SubscriptionClientName = configuration.SubscriptionClientName;
            });
        else if (configuration.Vendor == GOOGLE_CLOUD)
            services.AddGoogleCloudPubSub(options =>
            {
                options.ProjectId = configuration.GcpPubSubProjectId;
                options.TopicName = configuration.GcpPubSubTopicName;
                options.SubscriptionClientName = configuration.SubscriptionClientName;
                options.ConnectionInfo = configuration.ConnectionInfo;
            });
        else if (configuration.Vendor == RABBIT_MQ)
            services.AddRabbitMQ(options =>
            {
                options.HostName = configuration.ConnectionInfo;
                options.Username = configuration.RabbitMQUsername;
                options.Password = configuration.RabbitMQPassword;
                options.SubscriptionClientName = configuration.SubscriptionClientName;
                options.RetryCount = configuration.ConnectionRetryCount;
            });
        else if (configuration.Vendor.IsEmpty())
            throw new NotSupportedException("No event bus vendor was specified.");
        else
            throw new NotSupportedException(
                $"{configuration.Vendor} is not a currently supported event bus vendor.");
    }
}

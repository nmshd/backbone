namespace Backbone.Infrastructure.EventBus;

public class EventBusConfiguration
{
    public string Vendor { get; set; }

    public string ConnectionInfo { get; set; }

    public string SubscriptionClientName { get; set; }

    public string RabbitMQUsername { get; set; }
    public string RabbitMQPassword { get; set; }
    public int ConnectionRetryCount { get; set; }

    public string GcpPubSubProjectId { get; set; }
    public string GcpPubSubTopicName { get; set; }
}
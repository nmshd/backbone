using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Tokens.Infrastructure.EventBus;

public class EventBusConfiguration
{
    public string RabbitMQUsername { get; set; }
    public string RabbitMQPassword { get; set; }

    public int ConnectionRetryCount { get; set; }
    public string SubscriptionClientName { get; set; }

    public string GcpPubSubProjectId { get; set; }
    public string GcpPubSubTopicName { get; set; }
    public string ConnectionInfo { get; set; }

    [RegularExpression("Azure|GoogleCloud")]
    public string Vendor { get; set; }
}

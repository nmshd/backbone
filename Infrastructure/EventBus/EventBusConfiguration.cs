using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.Infrastructure.EventBus;

public class EventBusConfiguration
{
    [Required]
    [RegularExpression("Azure|GoogleCloud|RabbitMQ")]
    public string Vendor { get; set; } = null!;

    public string ConnectionInfo { get; set; } = null!;

    [Required]
    public string SubscriptionClientName { get; set; } = null!;

    public string RabbitMqUsername { get; set; } = null!;
    public string RabbitMqPassword { get; set; } = null!;
    public int ConnectionRetryCount { get; set; }

    public string GcpPubSubProjectId { get; set; } = null!;
    public string GcpPubSubTopicName { get; set; } = null!;

    public HandlerRetryBehavior HandlerRetryBehavior { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;

namespace Backbone.Infrastructure.EventBus;

public class EventBusConfiguration
{
    [Required]
    [RegularExpression("Azure|GoogleCloud|RabbitMQ")]
    public string Vendor { get; set; }

    public string ConnectionInfo { get; set; }

    [Required]
    public string SubscriptionClientName { get; set; }

    public string RabbitMqUsername { get; set; }
    public string RabbitMqPassword { get; set; }
    public int ConnectionRetryCount { get; set; }

    public string GcpPubSubProjectId { get; set; }
    public string GcpPubSubTopicName { get; set; }

    public uint NumberOfRetries { get; set; } = 5;

    /// <summary>
    /// in milliseconds.
    /// </summary>
    public uint MinimumBackoff { get; set; } = 500;

    /// <summary>
    /// in seconds.
    /// </summary>
    public uint MaximumBackoff { get; set; } = 120;
}

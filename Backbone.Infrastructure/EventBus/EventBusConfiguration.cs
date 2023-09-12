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

    [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
    public int NumberOfRetries { get; set; } = 5;

    [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
    public int MinimumBackoff { get; set; } = 2;

    [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
    public int MaximumBackoff { get; set; } = 120;
}

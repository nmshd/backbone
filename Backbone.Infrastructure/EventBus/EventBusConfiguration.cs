﻿using System.ComponentModel.DataAnnotations;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus;

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

    public HandlerRetryBehavior HandlerRetryBehavior { get; set; }
}

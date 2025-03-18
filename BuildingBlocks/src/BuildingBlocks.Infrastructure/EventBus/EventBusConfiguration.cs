using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public class EventBusConfiguration
{
    [Required]
    [RegularExpression("AzureServiceBus|GoogleCloudPubSub|RabbitMQ")]
    public string ProductName { get; set; } = null!;

    public RabbitMqConfiguration RabbitMq { get; set; } = null!;

    public ServiceBusOptions AzureServiceBus { get; set; } = null!;

    public GoogleCloudPubSubConfiguration GoogleCloudPubSub { get; set; } = null!;
}

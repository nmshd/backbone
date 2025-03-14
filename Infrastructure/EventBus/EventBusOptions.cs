using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

namespace Backbone.Infrastructure.EventBus;

public class EventBusOptions
{
    [Required]
    [RegularExpression("AzureServiceBus|GoogleCloudPubSub|RabbitMQ")]
    public string ProductName { get; set; } = null!;

    public RabbitMqOptions RabbitMq { get; set; } = null!;

    public ServiceBusOptions AzureServiceBus { get; set; } = null!;

    public GoogleCloudPubSubOptions GoogleCloudPubSub { get; set; } = null!;
}

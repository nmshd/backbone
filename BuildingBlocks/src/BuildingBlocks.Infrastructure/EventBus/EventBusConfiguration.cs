using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public class EventBusConfiguration
{
    public const string AZURE = "AzureServiceBus";
    public const string GOOGLE_CLOUD = "GoogleCloudPubSub";
    public const string RABBIT_MQ = "RabbitMQ";

    [Required]
    [RegularExpression($"{AZURE}|{GOOGLE_CLOUD}|{RABBIT_MQ}")]
    public required string ProductName { get; init; }


    [RequiredIf(nameof(ProductName), RABBIT_MQ)]
    public RabbitMqConfiguration RabbitMq { get; init; } = null!;


    [RequiredIf(nameof(ProductName), AZURE)]
    public ServiceBusOptions AzureServiceBus { get; init; } = null!;


    [RequiredIf(nameof(ProductName), GOOGLE_CLOUD)]
    public GoogleCloudPubSubConfiguration GoogleCloudPubSub { get; init; } = null!;
}

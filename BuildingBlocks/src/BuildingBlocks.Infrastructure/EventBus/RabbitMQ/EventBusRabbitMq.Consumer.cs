using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public partial class EventBusRabbitMq
{
    private const int HANDLER_RETRY_COUNT = 5;
    private const string PROCESS_OPERATION_NAME = "consume";
    private const string PROCESS_OPERATION_TYPE = "process";

    private readonly string _deadLetterExchangeName;
    private readonly string _deadLetterQueueName;
    private readonly SubscriptionManager _subscriptionManager = new();

    public async Task Subscribe<TEvent, THandler>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var queueName = GetQueueName<THandler, TEvent>();

        await CreateQueue<TEvent>(queueName);

        var consumer = await CreateConsumer<TEvent, THandler>();

        _subscriptionManager.AddSubscription(consumer, queueName);
    }

    private async Task EnsureDeadLetterQueueExists()
    {
        IChannel? channel = null;

        try
        {
            channel = await _channelPool.Get();

            await channel.QueueDeclareAsync(_deadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    { "x-queue-type", "quorum" },
                }
            );

            await channel.QueueBindAsync(_deadLetterQueueName, _deadLetterExchangeName, "#");

            _logger.LogTrace("Successfully bound dead letter queue.");
        }
        finally
        {
            if (channel != null)
                _channelPool.Return(channel);
        }
    }

    private async Task CreateQueue<TEvent>(string queueName) where TEvent : DomainEvent
    {
        var eventName = typeof(TEvent).GetEventName();

        IChannel? channel = null;

        try
        {
            channel = await _channelPool.Get();

            await channel.QueueDeclareAsync(queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    { "x-queue-type", "quorum" },
                    { "x-delivery-limit", HANDLER_RETRY_COUNT },
                    { "x-dead-letter-exchange", _deadLetterExchangeName },
                    { "x-dead-letter-routing-key", $"dead.routing.{eventName}" }
                }
            );

            await channel.QueueBindAsync(queueName, _exchangeName, eventName);

            _logger.LogTrace("Successfully bound queue '{QueueName}' to event '{EventName}'.", queueName, eventName);
        }
        finally
        {
            if (channel != null)
                _channelPool.Return(channel);
        }
    }

    private async Task<AsyncEventingBasicConsumer> CreateConsumer<TEvent, THandler>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var channel = await _connection.CreateChannelAsync();
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var queueName = GetQueueName<THandler, TEvent>();

            using var activity = StartProcessActivity(eventArgs, queueName, eventArgs.BasicProperties, eventArgs.Body.Length);

            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            activity?.AddEvent(new ActivityEvent("enmeshed.backbone.event_bus.consumer.message_decoded"));

            try
            {
                var correlationId = eventArgs.BasicProperties.CorrelationId;
                correlationId = correlationId.IsNullOrEmpty() ? CustomLogContext.GenerateCorrelationId() : correlationId;

                using (CustomLogContext.SetCorrelationId(correlationId))
                {
                    await ProcessEvent<TEvent, THandler>(message);

                    await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);

                await channel.BasicRejectAsync(eventArgs.DeliveryTag, true);
                _metrics.IncrementNumberOfProcessingErrors(GetQueueName<THandler, TEvent>());
            }
        };

        return consumer;
    }

    private async Task ProcessEvent<TEvent, THandler>(string message) where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        Activity.Current?.AddEvent(new ActivityEvent("enmeshed.backbone.event_bus.consumer.start_processing"));

        var domainEvent = JsonSerializer.Deserialize<TEvent>(message);

        var handlerType = typeof(THandler);

        await using var scope = _serviceProvider.CreateAsyncScope();

        if (scope.ServiceProvider.GetService(handlerType) is not IDomainEventHandler handler)
            throw new Exception("Domain event handler could not be resolved from dependency container or it does not implement IDomainEventHandler.");

        Activity.Current?.AddEvent(new ActivityEvent("enmeshed.backbone.event_bus.consumer.handler_resolved"));

        var startedAt = Stopwatch.GetTimestamp();
        await (Task)handlerType.GetMethod("Handle")!.Invoke(handler, [domainEvent])!;
        _metrics.TrackEventProcessingDuration(startedAt, GetQueueName<THandler, TEvent>());

        _metrics.IncrementNumberOfHandledEvents(GetQueueName<THandler, TEvent>());
    }

    public async Task StartConsuming(CancellationToken cancellationToken)
    {
        foreach (var subscription in _subscriptionManager)
        {
            await subscription.Consumer.Channel.BasicConsumeAsync(subscription.QueueName, autoAck: false, subscription.Consumer, cancellationToken);
        }
    }

    public async Task StopConsuming(CancellationToken cancellationToken)
    {
        foreach (var consumerData in _subscriptionManager)
        {
            var channel = consumerData.Consumer.Channel;
            foreach (var tag in consumerData.Consumer.ConsumerTags)
            {
                await channel.BasicCancelAsync(tag, cancellationToken: cancellationToken);
            }
        }
    }

    private Activity? StartProcessActivity(BasicDeliverEventArgs eventArgs, string queueName, IReadOnlyBasicProperties properties, int bodySize)
    {
        var parentContext = EventBusDiagnostics.PROPAGATOR.Extract(default,
            eventArgs.BasicProperties,
            ExtractTraceContextFromBasicProperties);
        Baggage.Current = parentContext.Baggage;

        var destinationName = $"{_exchangeName}.{queueName}";
        var activity = EventBusDiagnostics.ACTIVITY_SOURCE.StartActivity($"{PROCESS_OPERATION_NAME} {destinationName}", ActivityKind.Consumer, parentContext.ActivityContext);

        if (activity == null)
            return null;

        activity.SetTag("messaging.system", MESSAGING_SYSTEM);
        activity.SetTag("messaging.operation.name", PROCESS_OPERATION_NAME);
        activity.SetTag("messaging.operation.type", PROCESS_OPERATION_TYPE);
        activity.SetTag("messaging.destination.name", destinationName);
        activity.SetTag("messaging.destination.template", $"{_exchangeName}:{{queueName}}");
        activity.SetTag("messaging.rabbitmq.destination.routing_key", eventArgs.RoutingKey);
        activity.SetTag("messaging.rabbitmq.message.delivery_tag", eventArgs.DeliveryTag);
        activity.SetTag("messaging.message.body.size", bodySize);

        if (!properties.MessageId.IsNullOrEmpty())
            activity.SetTag("messaging.message.id", properties.MessageId);

        if (!properties.CorrelationId.IsNullOrEmpty())
            activity.SetTag("messaging.message.conversation_id", properties.CorrelationId);

        return activity;
    }

    private IEnumerable<string> ExtractTraceContextFromBasicProperties(IReadOnlyBasicProperties props, string key)
    {
        if (props.Headers == null || !props.Headers.TryGetValue(key, out var value)) return [];

        if (value is byte[] bytes) return [Encoding.UTF8.GetString(bytes)];

        return [];
    }

    public static string GetQueueName<THandler, TEvent>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventHandlerFullName = typeof(THandler).FullName!;

        var moduleName = eventHandlerFullName.Split('.').ElementAt(2);

        return $"{moduleName}.{typeof(TEvent).GetEventName()}";
    }
}

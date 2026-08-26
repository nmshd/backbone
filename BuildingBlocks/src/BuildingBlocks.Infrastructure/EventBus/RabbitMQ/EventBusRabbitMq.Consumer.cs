using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public partial class EventBusRabbitMq
{
    public async Task Subscribe<TEvent, THandler>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var queueName = GetQueueName<THandler, TEvent>();

        await CreateQueue<TEvent>(queueName);

        var consumer = await CreateConsumer<TEvent, THandler>();

        _subscriptionManager.AddSubscription(consumer, queueName);
    }

    private async Task CreateQueue<TEvent>(string queueName) where TEvent : DomainEvent
    {
        var eventName = typeof(TEvent).GetEventName();

        var channel = await _channelPool.Get();

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

        _channelPool.Return(channel);
    }

    private async Task<AsyncEventingBasicConsumer> CreateConsumer<TEvent, THandler>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var channel = await _connection.CreateChannelAsync();
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var eventName = eventArgs.RoutingKey;

            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

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
                await channel.BasicRejectAsync(eventArgs.DeliveryTag, true);
                _metrics.IncrementNumberOfProcessingErrors(GetQueueName<THandler, TEvent>());

                _logger.ErrorWhileProcessingDomainEvent(eventName, ex);
            }
        };

        return consumer;
    }

    private async Task ProcessEvent<TEvent, THandler>(string message) where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);
        var eventName = eventType.GetEventName();

        _logger.LogDebug("Processing RabbitMQ event: '{EventName}'", eventName);

        var domainEvent = JsonSerializer.Deserialize<TEvent>(message);

        var handlerType = typeof(THandler);

        await using var scope = _serviceProvider.CreateAsyncScope();

        if (scope.ServiceProvider.GetService(handlerType) is not IDomainEventHandler handler)
            throw new Exception("Domain event handler could not be resolved from dependency container or it does not implement IDomainEventHandler.");

        var concreteType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        var startedAt = Stopwatch.GetTimestamp();
        await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, [domainEvent])!;
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
}

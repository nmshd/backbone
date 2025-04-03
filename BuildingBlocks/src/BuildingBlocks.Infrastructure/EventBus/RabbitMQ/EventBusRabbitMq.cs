using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Sockets;
using System.Text;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.BuildingBlocks.Infrastructure.EventBus.Json;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public class EventBusMetrics
{
    private readonly UpDownCounter<int> _activeHandlers;
    private readonly Counter<long> _numberOfHandledEvents;
    private readonly Histogram<double> _eventProcessingDuration;
    private readonly Histogram<int> _messageSizeBytes;

    public EventBusMetrics(Meter meter)
    {
        _numberOfHandledEvents = meter.CreateCounter<long>(name: "enmeshed_events_handled_total");
        _eventProcessingDuration = meter.CreateHistogram(name: "enmeshed_events_processing_duration_seconds", unit: "s", advice: new InstrumentAdvice<double>
        {
            HistogramBucketBoundaries = [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10, 30, 60, 180, 600]
        });
        _activeHandlers = meter.CreateUpDownCounter<int>(name: "enmeshed_events_active_handlers");
        _messageSizeBytes = meter.CreateHistogram<int>(name: "enmeshed_events_message_size_bytes", unit: "By");
    }

    public void TrackEventProcessingDuration(EventProcessingDurationStage stage, long startedAt, string eventName, string queueName)
    {
        _eventProcessingDuration.Record(
            Stopwatch.GetElapsedTime(startedAt).TotalSeconds,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("queue_name", queueName),
            new KeyValuePair<string, object?>("stage", EventProcessingDurationStageToString(stage)));
    }

    public void IncrementNumberOfHandledEvents(string eventName, string queueName)
    {
        _numberOfHandledEvents.Add(
            1,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("queue_name", queueName)
        );
    }

    public void IncrementNumberOfActiveHandlers(string eventName, string queueName)
    {
        _activeHandlers.Add(
            1,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("queue_name", queueName)
        );
    }

    public void DecrementNumberOfActiveHandlers(string eventName, string queueName)
    {
        _activeHandlers.Add(
            -1,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("queue_name", queueName)
        );
    }

    public void TrackHandledMessageSize(int messageSize, string eventName)
    {
        _messageSizeBytes.Record(messageSize, new KeyValuePair<string, object?>("event_name", eventName));
    }

    private string EventProcessingDurationStageToString(EventProcessingDurationStage stage)
    {
        return stage switch
        {
            EventProcessingDurationStage.Deserialize => "deserialize",
            EventProcessingDurationStage.Handle => "handle",
            EventProcessingDurationStage.Reject => "reject",
            EventProcessingDurationStage.Acknowledge => "acknowledge",
            _ => "unknown"
        };
    }
}

public enum EventProcessingDurationStage
{
    Deserialize,
    Handle,
    Acknowledge,
    Reject
}

public class EventBusRabbitMq : IEventBus, IDisposable
{
    private const int PUBLISH_RETRY_COUNT = 5;
    private const int HANDLER_RETRY_COUNT = 5;

    private static readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new()
    {
        ContractResolver = new ContractResolverWithPrivates()
    };

    private readonly ILogger<EventBusRabbitMq> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRabbitMqPersistentConnection _persistentConnection;

    private readonly ChannelPool _channelPool;

    private readonly string _exchangeName;
    private readonly EventBusMetrics _metrics;
    private readonly string _deadLetterExchangeName;
    private readonly string _deadLetterQueueName;
    private readonly SubscriptionManager _subscriptionManager = new();

    private EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILogger<EventBusRabbitMq> logger, IServiceProvider serviceProvider, string exchangeName, EventBusMetrics metrics)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider;
        _exchangeName = exchangeName;
        _metrics = metrics;
        _deadLetterExchangeName = $"deadletterexchange.{exchangeName}";
        _deadLetterQueueName = $"deadletterqueue.{exchangeName}";
        _channelPool = new ChannelPool(persistentConnection);
    }

    public static async Task<EventBusRabbitMq> Create(IRabbitMqPersistentConnection persistentConnection, ILogger<EventBusRabbitMq> logger, IServiceProvider serviceProvider, string exchangeName,
        EventBusMetrics metrics)
    {
        var eventBus = new EventBusRabbitMq(persistentConnection, logger, serviceProvider, exchangeName, metrics);

        await eventBus.Init();

        return eventBus;
    }

    private async Task Init()
    {
        await ConnectToRabbitMq();
        await EnsureExchangeExists(_exchangeName);
        await EnsureExchangeExists(_deadLetterExchangeName, "fanout");
        await EnsureDeadLetterQueueExists();
    }

    private async Task ConnectToRabbitMq()
    {
        if (!_persistentConnection.IsConnected)
            await _persistentConnection.Connect();
    }

    private async Task EnsureExchangeExists(string exchangeName, string exchangeType = "direct")
    {
        try
        {
            var channel = await _channelPool.Get();
            await channel.ExchangeDeclarePassiveAsync(exchangeName);
            _channelPool.Return(channel);
        }
        catch (OperationInterruptedException ex)
        {
            if (ex.ShutdownReason?.ReplyCode == 404)
            {
                try
                {
                    var channel = await _channelPool.Get();
                    await channel.ExchangeDeclareAsync(exchangeName, exchangeType);
                    _channelPool.Return(channel);
                }
                catch (Exception)
                {
                    _logger.LogCritical("The exchange '{ExchangeName}' does not exist and could not be created.", exchangeName);
                    throw new Exception($"The exchange '{exchangeName}' does not exist and could not be created.");
                }
            }
        }
    }

    private async Task EnsureDeadLetterQueueExists()
    {
        var channel = await _channelPool.Get();

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

        _channelPool.Return(channel);
    }

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
        var channel = await _persistentConnection.CreateChannel();
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var eventName = eventArgs.RoutingKey;

            _metrics.IncrementNumberOfActiveHandlers(eventName, GetQueueName<THandler, TEvent>());

            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            try
            {
                var correlationId = eventArgs.BasicProperties.CorrelationId;
                correlationId = correlationId.IsNullOrEmpty() ? CustomLogContext.GenerateCorrelationId() : correlationId;

                using (CustomLogContext.SetCorrelationId(correlationId))
                {
                    await ProcessEvent<TEvent, THandler>(message);

                    var startedAt = Stopwatch.GetTimestamp();
                    await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                    _metrics.TrackEventProcessingDuration(EventProcessingDurationStage.Acknowledge, startedAt, eventName, GetQueueName<THandler, TEvent>());
                }
            }
            catch (Exception ex)
            {
                var startedAt = Stopwatch.GetTimestamp();
                await channel.BasicRejectAsync(eventArgs.DeliveryTag, true);
                _metrics.TrackEventProcessingDuration(EventProcessingDurationStage.Reject, startedAt, eventName, GetQueueName<THandler, TEvent>());

                _logger.ErrorWhileProcessingDomainEvent(eventName, ex);
            }

            _metrics.DecrementNumberOfActiveHandlers(eventName, GetQueueName<THandler, TEvent>());
        };

        return consumer;
    }

    private async Task ProcessEvent<TEvent, THandler>(string message) where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);
        var eventName = eventType.GetEventName();

        _logger.LogDebug("Processing RabbitMQ event: '{EventName}'", eventName);

        var startedAt = Stopwatch.GetTimestamp();
        var domainEvent = JsonConvert.DeserializeObject<TEvent>(message, JSON_SERIALIZER_SETTINGS);
        _metrics.TrackEventProcessingDuration(EventProcessingDurationStage.Deserialize, startedAt, eventName, GetQueueName<THandler, TEvent>());

        var handlerType = typeof(THandler);

        await using var scope = _serviceProvider.CreateAsyncScope();

        if (scope.ServiceProvider.GetService(handlerType) is not IDomainEventHandler handler)
            throw new Exception("Domain event handler could not be resolved from dependency container or it does not implement IDomainEventHandler.");

        var concreteType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        startedAt = Stopwatch.GetTimestamp();
        await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, [domainEvent])!;
        _metrics.TrackEventProcessingDuration(EventProcessingDurationStage.Handle, startedAt, eventName, GetQueueName<THandler, TEvent>());

        _metrics.IncrementNumberOfHandledEvents(eventName, GetQueueName<THandler, TEvent>());
    }

    public async Task Publish(DomainEvent @event)
    {
        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(PUBLISH_RETRY_COUNT,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, _) => _logger.ErrorOnPublish(ex));

        var eventName = @event.GetEventName();

        _logger.LogInformation("Creating RabbitMQ channel to publish a '{EventName}'.", eventName);

        var message = JsonConvert.SerializeObject(@event, JSON_SERIALIZER_SETTINGS);

        var body = Encoding.UTF8.GetBytes(message);

        _metrics.TrackHandledMessageSize(body.Length, eventName);

        await policy.ExecuteAsync(async () =>
        {
            _logger.LogDebug("Publishing a '{EventName}' event to RabbitMQ.", eventName);

            var channel = await _channelPool.Get();
            var properties = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = @event.DomainEventId,
                CorrelationId = CustomLogContext.GetCorrelationId()
            };

            await channel.BasicPublishAsync(_exchangeName, eventName, mandatory: false, properties, body);

            _logger.PublishedDomainEvent();

            _channelPool.Return(channel);
        });
    }

    public async Task StartConsuming(CancellationToken cancellationToken)
    {
        foreach (var subscription in _subscriptionManager.Subscriptions)
        {
            await subscription.Consumer.Channel.BasicConsumeAsync(subscription.QueueName, autoAck: false, subscription.Consumer, cancellationToken);
        }
    }

    public async Task StopConsuming(CancellationToken cancellationToken)
    {
        foreach (var consumerData in _subscriptionManager.Subscriptions)
        {
            var channel = consumerData.Consumer.Channel;
            foreach (var tag in consumerData.Consumer.ConsumerTags)
            {
                await channel.BasicCancelAsync(tag, cancellationToken: cancellationToken);
            }
        }
    }

    public void Dispose()
    {
        _channelPool.Dispose();
    }

    public static string GetQueueName<THandler, TEvent>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventHandlerFullName = typeof(THandler).FullName!;

        var moduleName = eventHandlerFullName.Split('.').ElementAt(2);

        return $"{moduleName}.{typeof(TEvent).GetEventName()}";
    }
}

internal static partial class EventBusRabbitMqLogs
{
    [LoggerMessage(
        EventId = 411326,
        EventName = "EventBusRabbitMQ.ErrorOnPublish",
        Level = LogLevel.Warning,
        Message = "There was an error while trying to publish an event.")]
    public static partial void ErrorOnPublish(this ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 585231,
        EventName = "EventBusRabbitMQ.PublishedDomainEvent",
        Level = LogLevel.Debug,
        Message = "Successfully published the event.")]
    public static partial void PublishedDomainEvent(this ILogger logger);

    [LoggerMessage(
        EventId = 702822,
        EventName = "EventBusRabbitMQ.ErrorWhileProcessingDomainEvent",
        Level = LogLevel.Error,
        Message = "An error occurred while processing the domain event of type '{eventName}'.")]
    public static partial void ErrorWhileProcessingDomainEvent(this ILogger logger, string eventName, Exception exception);
}

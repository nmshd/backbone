using System.Collections.Concurrent;
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

public class NewEventBusRabbitMq : IEventBus, IDisposable
{
    private static readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new()
    {
        ContractResolver = new ContractResolverWithPrivates()
    };

    private readonly ILogger<NewEventBusRabbitMq> _logger;

    private readonly IServiceProvider _serviceProvider;

    private readonly IRabbitMqPersistentConnection _persistentConnection;
    private readonly int _connectionRetryCount;
    private readonly HandlerRetryBehavior _handlerRetryBehavior;
    private readonly ChannelPool _channelPool;

    private readonly string _exchangeName;
    private readonly IList<Subscription> _subscriptions = [];

    private NewEventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILogger<NewEventBusRabbitMq> logger,
        IServiceProvider serviceProvider, HandlerRetryBehavior handlerRetryBehavior, string exchangeName, int connectionRetryCount = 5)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider;
        _exchangeName = exchangeName;
        _connectionRetryCount = connectionRetryCount;
        _handlerRetryBehavior = handlerRetryBehavior;
        _channelPool = new ChannelPool(persistentConnection);
    }

    public static async Task<NewEventBusRabbitMq> Create(IRabbitMqPersistentConnection persistentConnection, ILogger<NewEventBusRabbitMq> logger,
        IServiceProvider serviceProvider, HandlerRetryBehavior handlerRetryBehavior, string exchangeName, int connectionRetryCount = 5)
    {
        var eventBus = new NewEventBusRabbitMq(persistentConnection, logger, serviceProvider, handlerRetryBehavior, exchangeName, connectionRetryCount);

        await eventBus.Init();

        return eventBus;
    }

    private async Task Init()
    {
        await ConnectToRabbitMq();
        await EnsureExchangeExists();
    }

    private async Task ConnectToRabbitMq()
    {
        if (!_persistentConnection.IsConnected)
            await _persistentConnection.Connect();
    }

    private async Task EnsureExchangeExists()
    {
        try
        {
            var channel = await _channelPool.Get();
            await channel.ExchangeDeclarePassiveAsync(_exchangeName);
            _channelPool.Return(channel);
        }
        catch (OperationInterruptedException ex)
        {
            if (ex.ShutdownReason?.ReplyCode == 404)
            {
                try
                {
                    var channel = await _channelPool.Get();
                    await channel.ExchangeDeclareAsync(_exchangeName, "direct");
                    _channelPool.Return(channel);
                }
                catch (Exception)
                {
                    _logger.LogCritical("The exchange '{ExchangeName}' does not exist and could not be created.", _exchangeName);
                    throw new Exception($"The exchange '{_exchangeName}' does not exist and could not be created.");
                }
            }
        }
    }

    public async Task Subscribe<TEvent, THandler>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var queueName = DomainEventNamingHelpers.GetQueueName<THandler, TEvent>();

        await CreateQueue<TEvent>(queueName);

        var consumer = await CreateConsumer<TEvent, THandler>();

        _subscriptions.Add(new Subscription(consumer, queueName));
    }

    private async Task CreateQueue<TEvent>(string queueName) where TEvent : DomainEvent
    {
        var channel = await _channelPool.Get();

        await channel.QueueDeclareAsync(queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                { "x-queue-type", "quorum" }
            }
        );

        var eventName = DomainEventNamingHelpers.GetEventName(typeof(TEvent));

        await channel.QueueBindAsync(queueName, _exchangeName, eventName);

        _logger.LogTrace("Successfully bound queue '{QueueName}' to event '{EventName}'.", queueName, eventName);

        _channelPool.Return(channel);
    }

    private async Task<AsyncEventingBasicConsumer> CreateConsumer<TEvent, THandler>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var channel = await _channelPool.Get();
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
                await channel.BasicRejectAsync(eventArgs.DeliveryTag, false);

                _logger.ErrorWhileProcessingDomainEvent(eventName, ex);
            }
        };

        _channelPool.Return(channel);

        return consumer;
    }

    private async Task ProcessEvent<TEvent, THandler>(string message) where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);
        var eventName = DomainEventNamingHelpers.GetEventName(eventType);

        _logger.LogDebug("Processing RabbitMQ event: '{EventName}'", eventName);

        var domainEvent = JsonConvert.DeserializeObject<TEvent>(message, JSON_SERIALIZER_SETTINGS);
        var policy = EventBusRetryPolicyFactory.Create(_handlerRetryBehavior, (ex, _) => _logger.ErrorWhileExecutingEventHandlerType(eventName, ex));

        var handlerType = typeof(THandler);

        await policy.ExecuteAsync(async () =>
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            if (scope.ServiceProvider.GetService(handlerType) is not IDomainEventHandler handler)
                throw new Exception("Domain event handler could not be resolved from dependency container or it does not implement IDomainEventHandler.");

            var concreteType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

            await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, [domainEvent])!;
        });
    }

    public async Task Publish(DomainEvent @event)
    {
        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(_connectionRetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, _) => _logger.ErrorOnPublish(ex));

        var eventName = @event.GetEventName();

        _logger.LogInformation("Creating RabbitMQ channel to publish a '{EventName}'.", eventName);

        var message = JsonConvert.SerializeObject(@event, JSON_SERIALIZER_SETTINGS);

        var body = Encoding.UTF8.GetBytes(message);

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
        foreach (var consumer in _subscriptions)
        {
            await consumer.Consumer.Channel.BasicConsumeAsync(consumer.QueueName, autoAck: false, consumer.Consumer, cancellationToken);
        }
    }

    public async Task StopConsuming(CancellationToken cancellationToken)
    {
        foreach (var consumerData in _subscriptions)
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
}

public static class DomainEventNamingHelpers
{
    public static string GetEventName<T>(this T @event) where T : DomainEvent
    {
        return GetEventName(@event.GetType());
    }

    public static string GetEventName(Type eventType)
    {
        return eventType.Name.Replace("DomainEvent", string.Empty);
    }

    public static string GetQueueName<THandler, TEvent>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventHandlerFullName = typeof(THandler).FullName!;

        var moduleName = eventHandlerFullName.Split('.').ElementAt(2);

        return $"{moduleName}.{GetEventName(typeof(TEvent))}";
    }
}

public class Subscription
{
    public Subscription(AsyncEventingBasicConsumer consumer, string queueName)
    {
        Consumer = consumer;
        QueueName = queueName;
    }

    public AsyncEventingBasicConsumer Consumer { get; set; }
    public string QueueName { get; set; }
}

public abstract class ObjectPool<T>
{
    private readonly ConcurrentBag<T> _objects = [];

    public async Task<T> Get()
    {
        return _objects.TryTake(out var item) ? item : await CreateObject();
    }

    public void Return(T item) => _objects.Add(item);

    protected abstract Task<T> CreateObject();
}

public class ChannelPool : ObjectPool<IChannel>, IDisposable
{
    private readonly IRabbitMqPersistentConnection _connection;

    public ChannelPool(IRabbitMqPersistentConnection connection)
    {
        _connection = connection;
    }

    protected override async Task<IChannel> CreateObject()
    {
        return await _connection.CreateChannel();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}

using System.Net.Sockets;
using System.Text;
using Autofac;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.BuildingBlocks.Infrastructure.EventBus.Json;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public class NewEventBusRabbitMq : IEventBus, IDisposable
{
    private const string AUTOFAC_SCOPE_NAME = "event_bus";

    private readonly ILifetimeScope _autofac;
    private readonly ILogger<NewEventBusRabbitMq> _logger;

    private readonly IRabbitMqPersistentConnection _persistentConnection;
    private readonly int _connectionRetryCount;
    private readonly HandlerRetryBehavior _handlerRetryBehavior;

    private readonly string _exchangeName;
    private readonly IList<ConsumerMetadata> _consumersData = [];

    private NewEventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILogger<NewEventBusRabbitMq> logger,
        ILifetimeScope autofac, HandlerRetryBehavior handlerRetryBehavior, string exchangeName, int connectionRetryCount = 5)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _exchangeName = exchangeName;
        _autofac = autofac;
        _connectionRetryCount = connectionRetryCount;
        _handlerRetryBehavior = handlerRetryBehavior;
    }

    public static async Task<NewEventBusRabbitMq> Create(IRabbitMqPersistentConnection persistentConnection, ILogger<NewEventBusRabbitMq> logger,
        ILifetimeScope autofac, HandlerRetryBehavior handlerRetryBehavior, string exchangeName, int connectionRetryCount = 5)
    {
        var eventBus = new NewEventBusRabbitMq(persistentConnection, logger, autofac, handlerRetryBehavior, exchangeName, connectionRetryCount);

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
            await using var channel = await _persistentConnection.CreateChannel();
            await channel.ExchangeDeclarePassiveAsync(_exchangeName);
        }
        catch (OperationInterruptedException ex)
        {
            if (ex.ShutdownReason?.ReplyCode == 404)
            {
                try
                {
                    await using var channel = await _persistentConnection.CreateChannel();
                    await channel.ExchangeDeclareAsync(_exchangeName, "direct");
                }
                catch (Exception)
                {
                    _logger.LogCritical("The exchange '{ExchangeName}' does not exist and could not be created.", _exchangeName);
                    throw new Exception($"The exchange '{_exchangeName}' does not exist and could not be created.");
                }
            }
        }
    }

    public async Task Subscribe<T, TH>() where T : DomainEvent where TH : IDomainEventHandler<T>
    {
        var queueName = DomainEventNamingHelpers.GetQueueName<TH, T>();

        await BindQueueToEvent<T>(queueName);

        await CreateConsumer<T, TH>(queueName);
    }

    private async Task BindQueueToEvent<T>(string queueName) where T : DomainEvent
    {
        await using var channel = await _persistentConnection.CreateChannel();

        await channel.QueueDeclareAsync(queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                { "x-queue-type", "quorum" }
            }
        );

        var eventName = DomainEventNamingHelpers.GetEventName(typeof(T));

        await channel.QueueBindAsync(queueName, _exchangeName, eventName);

        _logger.LogTrace("Successfully bound queue '{QueueName}' to event '{EventName}'.", queueName, eventName);
    }

    private async Task CreateConsumer<TEvent, THandler>(string queueName) where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var channel = await _persistentConnection.CreateChannel();
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            try
            {
                var correlationId = eventArgs.BasicProperties.CorrelationId;
                correlationId = correlationId.IsNullOrEmpty() ? Guid.NewGuid().ToString() : correlationId;

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

        _consumersData.Add(new ConsumerMetadata(consumer, queueName));
    }

    private async Task ProcessEvent<TEvent, THandler>(string message) where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);
        var eventName = DomainEventNamingHelpers.GetEventName(eventType);

        _logger.LogDebug("Processing RabbitMQ event: '{EventName}'", eventName);

        var domainEvent = JsonConvert.DeserializeObject<TEvent>(message,
            new JsonSerializerSettings
            {
                ContractResolver = new ContractResolverWithPrivates()
            });
        var policy = EventBusRetryPolicyFactory.Create(
            _handlerRetryBehavior, (ex, _) => _logger.ErrorWhileExecutingEventHandlerType(eventName, ex));

        var handlerType = typeof(THandler);

        await policy.ExecuteAsync(async () =>
        {
            await using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);

            if (scope.ResolveOptional(handlerType) is not IDomainEventHandler handler)
                throw new Exception(
                    "Domain event handler could not be resolved from dependency container or it does not implement IDomainEventHandler.");

            var concreteType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

            await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, [domainEvent])!;
        });
    }

    public async Task Publish(DomainEvent @event)
    {
        if (!_persistentConnection.IsConnected)
            await _persistentConnection.Connect();

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(_connectionRetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, _) => _logger.ErrorOnPublish(ex));

        var eventName = @event.GetEventName();

        _logger.LogInformation("Creating RabbitMQ channel to publish a '{EventName}'.", eventName);

        var message = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
        {
            ContractResolver = new ContractResolverWithPrivates()
        });

        var body = Encoding.UTF8.GetBytes(message);

        await policy.ExecuteAsync(async () =>
        {
            _logger.LogDebug("Publishing a '{EventName}' event to RabbitMQ.", eventName);

            // TODO: reuse channel instead of recreate them; but be careful to not share between different threads
            await using var channel = await _persistentConnection.CreateChannel();
            var properties = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = @event.DomainEventId,
                CorrelationId = CustomLogContext.GetCorrelationId()
            };

            await channel.BasicPublishAsync(_exchangeName, eventName, mandatory: false, properties, body);

            _logger.PublishedDomainEvent();
        });
    }

    public async Task StartConsuming(CancellationToken cancellationToken)
    {
        foreach (var consumer in _consumersData)
        {
            await consumer.Consumer.Channel.BasicConsumeAsync(consumer.QueueName, autoAck: false, consumer.Consumer, cancellationToken);
        }
    }

    public async Task StopConsuming(CancellationToken cancellationToken)
    {
        foreach (var consumerData in _consumersData)
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
        _autofac.Dispose();
        _persistentConnection.Dispose();
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

public class ConsumerMetadata
{
    public ConsumerMetadata(AsyncEventingBasicConsumer consumer, string queueName)
    {
        Consumer = consumer;
        QueueName = queueName;
    }
    /*
     * Consumer
     * Queue
     * EventName
     * QueueName
     *
     */

    public AsyncEventingBasicConsumer Consumer { get; set; }
    public string QueueName { get; set; }
}

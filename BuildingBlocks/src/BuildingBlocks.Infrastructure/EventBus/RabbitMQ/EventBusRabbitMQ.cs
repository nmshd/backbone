// using System.Net.Sockets;
// using System.Text;
// using Autofac;
// using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
// using Backbone.BuildingBlocks.Domain.Events;
// using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
// using Backbone.BuildingBlocks.Infrastructure.EventBus.Json;
// using Backbone.Tooling.Extensions;
// using Microsoft.Extensions.Logging;
// using Newtonsoft.Json;
// using Polly;
// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;
// using RabbitMQ.Client.Exceptions;
//
// namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
//
// public class EventBusRabbitMq : IEventBus, IDisposable
// {
//     private const string AUTOFAC_SCOPE_NAME = "event_bus";
//
//     private readonly ILifetimeScope _autofac;
//     private readonly ILogger<EventBusRabbitMq> _logger;
//
//     private readonly IRabbitMqPersistentConnection _persistentConnection;
//     private readonly int _connectionRetryCount;
//     private readonly HandlerRetryBehavior _handlerRetryBehavior;
//     private readonly IEventBusSubscriptionsManager _subsManager;
//
//     private readonly string _consumerChannelTag = Guid.NewGuid().ToString("N");
//     private IChannel? _consumerChannel;
//     private readonly string _exchangeName;
//     private readonly string _queueName;
//     private AsyncEventingBasicConsumer? _consumer;
//     private bool _exchangeExistenceEnsured;
//     private bool _queueExistenceEnsured;
//
//     public EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILogger<EventBusRabbitMq> logger,
//         ILifetimeScope autofac, IEventBusSubscriptionsManager? subsManager, HandlerRetryBehavior handlerRetryBehavior, string exchangeName, string queueName,
//         int connectionRetryCount = 5)
//     {
//         _persistentConnection =
//             persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
//         _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//         _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
//         _exchangeName = exchangeName;
//         _queueName = queueName;
//         _autofac = autofac;
//         _connectionRetryCount = connectionRetryCount;
//         _handlerRetryBehavior = handlerRetryBehavior;
//     }
//
//     public void Dispose()
//     {
//         _consumerChannel?.Dispose();
//         _subsManager.Clear();
//     }
//
//     public async Task StartConsuming(CancellationToken cancellationToken)
//     {
//         if (_consumer is null)
//         {
//             throw new Exception("Cannot start consuming without a consumer set.");
//         }
//
//         await _consumerChannel!.BasicConsumeAsync(_queueName, false, _consumerChannelTag, _consumer, cancellationToken);
//     }
//
//     public async Task Publish(DomainEvent @event)
//     {
//         if (!_persistentConnection.IsConnected)
//             await _persistentConnection.Connect();
//
//         var policy = Policy.Handle<BrokerUnreachableException>()
//             .Or<SocketException>()
//             .WaitAndRetryAsync(_connectionRetryCount,
//                 retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
//                 (ex, _) => _logger.ErrorOnPublish(ex));
//
//         var eventName = @event.GetType().Name;
//
//         _logger.LogInformation("Creating RabbitMQ channel to publish a '{EventName}'.", eventName);
//
//         var message = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
//         {
//             ContractResolver = new ContractResolverWithPrivates()
//         });
//
//         var body = Encoding.UTF8.GetBytes(message);
//
//         await policy.ExecuteAsync(async () =>
//         {
//             _logger.LogDebug("Publishing a {EventName} to RabbitMQ.", eventName);
//
//             await using var channel = await _persistentConnection.CreateChannel();
//             var properties = new BasicProperties
//             {
//                 DeliveryMode = DeliveryModes.Persistent,
//                 MessageId = @event.DomainEventId,
//                 CorrelationId = CustomLogContext.GetCorrelationId()
//             };
//
//             await channel.BasicPublishAsync(_exchangeName,
//                 eventName,
//                 true,
//                 properties,
//                 body);
//
//             _logger.PublishedDomainEvent();
//         });
//     }
//
//     private async Task EnsureExchangeExists()
//     {
//         if (_exchangeExistenceEnsured)
//             return;
//
//         try
//         {
//             await using var channel = await _persistentConnection.CreateChannel();
//             await channel.ExchangeDeclarePassiveAsync(_exchangeName);
//             _exchangeExistenceEnsured = true;
//         }
//         catch (OperationInterruptedException ex)
//         {
//             if (ex.ShutdownReason?.ReplyCode == 404)
//             {
//                 try
//                 {
//                     await using var channel = await _persistentConnection.CreateChannel();
//                     await channel.ExchangeDeclareAsync(_exchangeName, "direct");
//                     _exchangeExistenceEnsured = true;
//                 }
//                 catch (Exception)
//                 {
//                     _logger.LogCritical("The exchange '{ExchangeName}' does not exist and could not be created.", _exchangeName);
//                     throw new Exception($"The exchange '{_exchangeName}' does not exist and could not be created.");
//                 }
//             }
//         }
//     }
//
//     public async Task Subscribe<T, TH>() where T : DomainEvent where TH : IDomainEventHandler<T>
//     {
//         var eventName = _subsManager.GetEventKey<T>();
//         await DoInternalSubscription(eventName);
//
//         _logger.LogInformation("Subscribing to event '{EventName}' with {EventHandler}", eventName, typeof(TH).Name);
//
//         _subsManager.AddSubscription<T, TH>();
//     }
//
//     private async Task DoInternalSubscription(string eventName)
//     {
//         await EnsureConsumerChannelExists();
//
//         var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
//         if (containsKey)
//         {
//             _logger.LogInformation("The messaging entity '{eventName}' already exists.", eventName);
//             return;
//         }
//
//         _logger.LogTrace("Trying to bind queue '{QueueName}' on RabbitMQ ...", _queueName);
//
//         await EnsureExchangeExists();
//
//         await _consumerChannel!.QueueBindAsync(_queueName, _exchangeName, eventName);
//
//         _logger.LogTrace("Successfully bound queue '{QueueName}' on RabbitMQ.", _queueName);
//     }
//
//     private async Task EnsureConsumerChannelExists()
//     {
//         if (_consumerChannel is null)
//         {
//             await CreateConsumerChannel();
//         }
//     }
//
//     private async Task CreateConsumerChannel()
//     {
//         if (!_persistentConnection.IsConnected)
//             await _persistentConnection.Connect();
//
//         _logger.LogInformation("Creating RabbitMQ consumer channel");
//
//         _consumerChannel = await _persistentConnection.CreateChannel();
//
//         await EnsureExchangeExists();
//
//         await EnsureQueueExists();
//
//         await _consumerChannel.QueueDeclareAsync(_queueName,
//             durable: true,
//             exclusive: false,
//             autoDelete: false,
//             arguments: new Dictionary<string, object?>
//             {
//                 { "x-queue-type", "quorum" }
//             }
//         );
//
//         _consumer = new AsyncEventingBasicConsumer(_consumerChannel);
//         _consumer.ReceivedAsync += async (_, eventArgs) =>
//         {
//             var eventName = eventArgs.RoutingKey;
//             var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
//
//             try
//             {
//                 var correlationId = eventArgs.BasicProperties.CorrelationId;
//                 correlationId = correlationId.IsNullOrEmpty() ? Guid.NewGuid().ToString() : correlationId;
//
//                 using (CustomLogContext.SetCorrelationId(correlationId))
//                 {
//                     await ProcessEvent(eventName, message);
//
//                     await _consumerChannel.BasicAckAsync(eventArgs.DeliveryTag, false);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 await _consumerChannel.BasicRejectAsync(eventArgs.DeliveryTag, false);
//
//                 _logger.ErrorWhileProcessingDomainEvent(eventName, ex);
//             }
//         };
//
//         _consumerChannel.CallbackExceptionAsync += async (_, ea) =>
//         {
//             _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");
//
//             _consumerChannel?.Dispose();
//             await CreateConsumerChannel();
//         };
//     }
//
//     private async Task EnsureQueueExists()
//     {
//         if (_queueExistenceEnsured)
//             return;
//
//         try
//         {
//             await using var channel = await _persistentConnection.CreateChannel();
//             await channel.QueueDeclarePassiveAsync(_queueName);
//             _queueExistenceEnsured = true;
//         }
//         catch (OperationInterruptedException ex)
//         {
//             if (ex.ShutdownReason?.ReplyCode == 404)
//             {
//                 try
//                 {
//                     await using var channel = await _persistentConnection.CreateChannel();
//
//                     await channel.QueueDeclareAsync(_queueName,
//                         durable: true,
//                         exclusive: false,
//                         autoDelete: false,
//                         arguments: new Dictionary<string, object?>
//                         {
//                             { "x-queue-type", "quorum" }
//                         }
//                     );
//
//                     _queueExistenceEnsured = true;
//                 }
//                 catch (Exception)
//                 {
//                     _logger.LogCritical("The queue '{QueueName}' does not exist and could not be created.", _queueName);
//                     throw new Exception($"The queue '{_queueName}' does not exist and could not be created.");
//                 }
//             }
//         }
//     }
//
//     private async Task ProcessEvent(string eventName, string message)
//     {
//         _logger.LogDebug("Processing RabbitMQ event: '{EventName}'", eventName);
//
//         if (_subsManager.HasSubscriptionsForEvent(eventName))
//         {
//             var subscriptions = _subsManager.GetHandlersForEvent(eventName);
//             foreach (var subscription in subscriptions)
//             {
//                 var eventType = subscription.EventType;
//
//                 if (eventType == null) throw new Exception($"Unsupported event type '${eventType}' received.");
//
//                 var domainEvent = JsonConvert.DeserializeObject(message, eventType,
//                     new JsonSerializerSettings
//                     {
//                         ContractResolver = new ContractResolverWithPrivates()
//                     });
//                 var policy = EventBusRetryPolicyFactory.Create(
//                     _handlerRetryBehavior, (ex, _) => _logger.ErrorWhileExecutingEventHandlerType(eventName, ex));
//
//                 await policy.ExecuteAsync(async () =>
//                 {
//                     await using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);
//
//                     if (scope.ResolveOptional(subscription.HandlerType) is not IDomainEventHandler handler)
//                         throw new Exception(
//                             "Domain event handler could not be resolved from dependency container or it does not implement IDomainEventHandler.");
//
//                     var concreteType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
//
//                     await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, [domainEvent])!;
//                 });
//             }
//         }
//         else
//         {
//             _logger.NoSubscriptionForEvent(eventName);
//         }
//     }
//
//     public async Task StopConsuming(CancellationToken cancellationToken)
//     {
//         if (_consumer is null)
//             return;
//
//         await _consumerChannel!.BasicCancelAsync(_consumerChannelTag, cancellationToken: cancellationToken);
//     }
// }



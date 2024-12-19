using System.Text;
using Autofac;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.BuildingBlocks.Infrastructure.EventBus.Json;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public class EventBusAzureServiceBus : IEventBus, IDisposable, IAsyncDisposable
{
    private const string DOMAIN_EVENT_SUFFIX = "DomainEvent";
    private const string TOPIC_NAME = "default";
    private const string AUTOFAC_SCOPE_NAME = "event_bus";
    private readonly ILifetimeScope _autofac;
    private readonly ILogger<EventBusAzureServiceBus> _logger;
    private readonly ServiceBusProcessor _processor;
    private readonly HandlerRetryBehavior _handlerRetryBehavior;
    private readonly ServiceBusSender _sender;
    private readonly IServiceBusPersisterConnection _serviceBusPersisterConnection;
    private readonly IEventBusSubscriptionsManager _subscriptionManager;
    private readonly string _subscriptionName;

    public EventBusAzureServiceBus(IServiceBusPersisterConnection serviceBusPersisterConnection,
        ILogger<EventBusAzureServiceBus> logger, IEventBusSubscriptionsManager subscriptionManager,
        ILifetimeScope autofac, HandlerRetryBehavior handlerRetryBehavior,
        string subscriptionClientName)
    {
        _serviceBusPersisterConnection = serviceBusPersisterConnection;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subscriptionManager = subscriptionManager;
        _autofac = autofac;
        _subscriptionName = subscriptionClientName;
        _sender = _serviceBusPersisterConnection.TopicClient.CreateSender(TOPIC_NAME);
        var options = new ServiceBusProcessorOptions { MaxConcurrentCalls = 10, AutoCompleteMessages = false };
        _processor = _serviceBusPersisterConnection.TopicClient.CreateProcessor(TOPIC_NAME, _subscriptionName, options);
        _handlerRetryBehavior = handlerRetryBehavior;
    }

    public void Dispose()
    {
        Task.Run(async () => await DisposeAsync()).GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        _subscriptionManager.Clear();
        await _processor.CloseAsync();
    }

    public async Task Publish(DomainEvent @event)
    {
        var eventName = @event.GetType().Name.Replace(DOMAIN_EVENT_SUFFIX, "");
        var jsonMessage = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
        {
            ContractResolver = new ContractResolverWithPrivates()
        });
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        var message = new ServiceBusMessage
        {
            MessageId = @event.DomainEventId,
            Body = new BinaryData(body),
            Subject = eventName,
            CorrelationId = CustomLogContext.GetCorrelationId()
        };

        _logger.SendingDomainEvent(message.MessageId);

        await _logger.TraceTime(async () =>
            await _sender.SendMessageAsync(message), nameof(_sender.SendMessageAsync));

        _logger.LogDebug("Successfully sent domain event with id '{MessageId}'.", message.MessageId);
    }

    public async Task Subscribe<T, TH>()
        where T : DomainEvent
        where TH : IDomainEventHandler<T>
    {
        var eventName = typeof(T).Name.Replace(DOMAIN_EVENT_SUFFIX, "");

        var containsKey = _subscriptionManager.HasSubscriptionsForEvent<T>();
        if (!containsKey)
            try
            {
                _logger.LogInformation("Trying to create subscription on Service Bus...");

                await _serviceBusPersisterConnection.AdministrationClient.CreateRuleAsync(TOPIC_NAME, _subscriptionName,
                    new CreateRuleOptions
                    {
                        Filter = new CorrelationRuleFilter { Subject = eventName },
                        Name = eventName
                    });

                _logger.LogInformation("Successfully created subscription on Service Bus.");
            }
            catch (ServiceBusException)
            {
                _logger.LogInformation("The messaging entity '{eventName}' already exists.", eventName);
            }

        _logger.LogInformation("Subscribing to event '{EventName}' with {EventHandler}", eventName, typeof(TH).Name);

        _subscriptionManager.AddSubscription<T, TH>();
    }

    public async Task StartConsuming()
    {
        await RegisterSubscriptionClientMessageHandlerAsync();
    }

    private async Task RegisterSubscriptionClientMessageHandlerAsync()
    {
        _processor.ProcessMessageAsync +=
            async args =>
            {
                var eventName = $"{args.Message.Subject}{DOMAIN_EVENT_SUFFIX}";
                var messageData = args.Message.Body.ToString();
                var correlationId = args.Message.CorrelationId;

                correlationId = correlationId.IsNullOrEmpty() ? CustomLogContext.GenerateCorrelationId() : correlationId;

                using (CustomLogContext.SetCorrelationId(correlationId))
                {
                    // Complete the message so that it is not received again.
                    if (await ProcessEvent(eventName, messageData))
                        await args.CompleteMessageAsync(args.Message);
                    else
                        _logger.EventWasNotProcessed(args.Message.MessageId);
                }
            };

        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        var ex = args.Exception;
        var context = args.ErrorSource;

        _logger.ErrorHandlingMessage(context, ex);

        return Task.CompletedTask;
    }

    private async Task<bool> ProcessEvent(string eventName, string message)
    {
        if (!_subscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            _logger.NoSubscriptionForEvent(eventName);
            return false;
        }

        var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);
        foreach (var subscription in subscriptions)
        {
            var eventType = subscription.EventType;
            var domainEvent = (DomainEvent)JsonConvert.DeserializeObject(message, eventType,
                new JsonSerializerSettings
                {
                    ContractResolver = new ContractResolverWithPrivates()
                })!;
            var concreteType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

            try
            {
                var policy = EventBusRetryPolicyFactory.Create(
                    _handlerRetryBehavior, (ex, _) => _logger.ErrorWhileExecutingEventHandlerType(eventType.Name, ex));

                await policy.ExecuteAsync(async () =>
                {
                    await using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);

                    if (scope.ResolveOptional(subscription.HandlerType) is not IDomainEventHandler handler)
                        throw new Exception(
                            "Domain event handler could not be resolved from dependency container or it does not implement IDomainEventHandler.");

                    await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, [domainEvent])!;
                });
            }
            catch (Exception ex)
            {
                _logger.ErrorWhileProcessingDomainEvent(eventName, ex);
                return false;
            }
        }

        return true;
    }
}

internal static partial class EventBusAzureServiceBusLogs
{
    [LoggerMessage(
        EventId = 302940,
        EventName = "EventBusAzureServiceBus.SendingDomainEvent",
        Level = LogLevel.Debug,
        Message = "Sending domain event with id '{messageId}'...")]
    public static partial void SendingDomainEvent(this ILogger logger, string messageId);

    [LoggerMessage(
        EventId = 630568,
        EventName = "EventBusAzureServiceBus.EventWasNotProcessed",
        Level = LogLevel.Information,
        Message = "The event with the MessageId '{messageId}' wasn't processed and will therefore not be completed.")]
    public static partial void EventWasNotProcessed(this ILogger logger, string messageId);

    [LoggerMessage(
        EventId = 949322,
        EventName = "EventBusAzureServiceBus.ErrorHandlingMessage",
        Level = LogLevel.Error,
        Message = "Error handling message with context {exceptionContext}.")]
    public static partial void ErrorHandlingMessage(this ILogger logger, ServiceBusErrorSource exceptionContext, Exception exception);

    [LoggerMessage(
        EventId = 341537,
        EventName = "EventBusAzureServiceBus.NoSubscriptionForEvent",
        Level = LogLevel.Warning,
        Message = "No subscription for event: '{eventName}'.")]
    public static partial void NoSubscriptionForEvent(this ILogger logger, string eventName);

    [LoggerMessage(
        EventId = 726744,
        EventName = "EventBusAzureServiceBus.ErrorWhileExecutingEventHandlerCausingRetry",
        Level = LogLevel.Warning,
        Message = "An error was thrown while executing '{eventHandlerType}'. Attempting to retry...")]
    public static partial void ErrorWhileExecutingEventHandlerType(this ILogger logger, string eventHandlerType, Exception exception);

    [LoggerMessage(
        EventId = 146670,
        EventName = "EventBusAzureServiceBus.ErrorWhileProcessingDomainEvent",
        Level = LogLevel.Error,
        Message = "An error occurred while processing the '{domainEventName}'.")]
    public static partial void ErrorWhileProcessingDomainEvent(this ILogger logger, string domainEventName, Exception ex);
}

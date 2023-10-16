using System.Text;
using Autofac;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public class EventBusAzureServiceBus : IEventBus, IDisposable
{
    private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";
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
        ILifetimeScope autofac,
        HandlerRetryBehavior handlerRetryBehavior,
        string subscriptionClientName)
    {
        _serviceBusPersisterConnection = serviceBusPersisterConnection;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subscriptionManager = subscriptionManager;
        _autofac = autofac;
        _subscriptionName = subscriptionClientName;
        _sender = _serviceBusPersisterConnection.TopicClient.CreateSender(TOPIC_NAME);
        var options = new ServiceBusProcessorOptions { MaxConcurrentCalls = 10, AutoCompleteMessages = false };
        _processor =
            _serviceBusPersisterConnection.TopicClient.CreateProcessor(TOPIC_NAME, _subscriptionName, options);

        _handlerRetryBehavior = handlerRetryBehavior;
    }

    public void Dispose()
    {
        _subscriptionManager.Clear();
        _processor.CloseAsync().GetAwaiter().GetResult();
    }

    public async void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFFIX, "");
        var jsonMessage = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
        {
            ContractResolver = new ContractResolverWithPrivates()
        });
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        var message = new ServiceBusMessage
        {
            MessageId = @event.IntegrationEventId,
            Body = new BinaryData(body),
            Subject = eventName
        };

        _logger.SendingIntegrationEvent(message.MessageId);

        await _logger.TraceTime(async () =>
            await _sender.SendMessageAsync(message), nameof(_sender.SendMessageAsync));

        _logger.LogDebug("Successfully sent integration event with id '{MessageId}'.", message.MessageId);
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");

        var containsKey = _subscriptionManager.HasSubscriptionsForEvent<T>();
        if (!containsKey)
            try
            {
                _logger.LogInformation("Trying to create subscription on Service Bus...");

                _serviceBusPersisterConnection.AdministrationClient.CreateRuleAsync(TOPIC_NAME, _subscriptionName,
                    new CreateRuleOptions
                    {
                        Filter = new CorrelationRuleFilter { Subject = eventName },
                        Name = eventName
                    }).GetAwaiter().GetResult();

                _logger.LogInformation("Successfully created subscription on Service Bus.");
            }
            catch (ServiceBusException)
            {
                _logger.LogInformation("The messaging entity '{eventName}' already exists.", eventName);
            }

        _logger.LogInformation("Subscribing to event '{EventName}' with {EventHandler}", eventName, typeof(TH).Name);

        _subscriptionManager.AddSubscription<T, TH>();
    }

    public void StartConsuming()
    {
        RegisterSubscriptionClientMessageHandlerAsync().GetAwaiter().GetResult();
    }

    private async Task RegisterSubscriptionClientMessageHandlerAsync()
    {
        _processor.ProcessMessageAsync +=
            async args =>
            {
                var eventName = $"{args.Message.Subject}{INTEGRATION_EVENT_SUFFIX}";
                var messageData = args.Message.Body.ToString();

                // Complete the message so that it is not received again.
                if (await ProcessEvent(eventName, messageData))
                    await args.CompleteMessageAsync(args.Message);
                else
                    _logger.EventWasNotProcessed(args.Message.MessageId);
            };

        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        var ex = args.Exception;
        var context = args.ErrorSource;

        _logger.ErrorHandlingMessage(ex.Message, context, ex);

        return Task.CompletedTask;
    }

    private async Task<bool> ProcessEvent(string eventName, string message)
    {
        if (!_subscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            _logger.NoSubscriptionForEvent(eventName);
            return false;
        }

        await using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);

        var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);
        foreach (var subscription in subscriptions)
        {
            var handler = scope.ResolveOptional(subscription.HandlerType);
            if (handler == null) continue;
            var eventType = subscription.EventType;
            var integrationEvent = (IntegrationEvent)JsonConvert.DeserializeObject(message, eventType,
                new JsonSerializerSettings
                {
                    ContractResolver = new ContractResolverWithPrivates()
                })!;
            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType!);

            try
            {
                var policy = EventBusRetryPolicyFactory.Create(
                    _handlerRetryBehavior, (ex, _) => _logger.ErrorWhileExecutingEventHandlerType(eventType.Name, ex.Message, ex.StackTrace!, ex));

                await policy.ExecuteAsync(() => (Task)concreteType.GetMethod("Handle")!.Invoke(handler, new[] { integrationEvent })!);
            }
            catch (Exception ex)
            {
                _logger.ErrorWhileProcessingIntegrationEvent(integrationEvent.IntegrationEventId, ex);
                return false;
            }
        }

        return true;
    }
}

file static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception> SENDING_INTEGRATION_EVENT =
        LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(302940, "EventBusAzureServiceBus.SendingIntegrationEvent"),
            "Sending integration event with id '{messageId}'..."
        );

    private static readonly Action<ILogger, string, Exception> EVENT_WAS_NOT_PROCESSED =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(630568, "EventBusAzureServiceBus.EventWasNotProcessed"),
            "The event with the MessageId '{messageId}' wasn't processed and will therefore not be completed."
        );

    private static readonly Action<ILogger, string, ServiceBusErrorSource, Exception> ERROR_HANDLING_MESSAGE =
        LoggerMessage.Define<string, ServiceBusErrorSource>(
            LogLevel.Error,
            new EventId(949322, "EventBusAzureServiceBus.ErrorHandlingMessage"),
            "ERROR handling message: '{exceptionMessage}' - Context: '{@exceptionContext}'."
        );

    private static readonly Action<ILogger, string, Exception> NO_SUBSCRIPTION_FOR_EVENT =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(341537, "EventBusAzureServiceBus.NoSubscriptionForEvent"),
            "No subscription for event: '{eventName}'."
        );

    private static readonly Action<ILogger, string, string, string, Exception> ERROR_WHILE_EXECUTING_EVENT_HANDLER_TYPE =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Warning,
            new EventId(726744, "EventBusAzureServiceBus.ErrorWhileExecutingEventHandlerCausingRetry"),
            "The following error was thrown while executing '{eventHandlerType}':\n'{errorMessage}'\n{stackTrace}.\nAttempting to retry..."
        );

    private static readonly Action<ILogger, string, Exception> ERROR_WHILE_PROCESSING_INTEGRATION_EVENT =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(146670, "EventBusAzureServiceBus.ErrorWhileProcessingIntegrationEvent"),
            "An error occurred while processing the integration event with id '{integrationEventId}'."
        );

    public static void SendingIntegrationEvent(this ILogger logger, string messageId)
    {
        SENDING_INTEGRATION_EVENT(logger, messageId, default!);
    }

    public static void EventWasNotProcessed(this ILogger logger, string messageId)
    {
        EVENT_WAS_NOT_PROCESSED(logger, messageId, default!);
    }

    public static void ErrorHandlingMessage(this ILogger logger, string exceptionMessage, ServiceBusErrorSource exceptionContext, Exception e)
    {
        ERROR_HANDLING_MESSAGE(logger, exceptionMessage, exceptionContext, e);
    }

    public static void NoSubscriptionForEvent(this ILogger logger, string eventName)
    {
        NO_SUBSCRIPTION_FOR_EVENT(logger, eventName, default!);
    }

    public static void ErrorWhileExecutingEventHandlerType(this ILogger logger, string eventHandlerType, string errorMessage, string stackTrace, Exception e)
    {
        ERROR_WHILE_EXECUTING_EVENT_HANDLER_TYPE(logger, eventHandlerType, errorMessage, stackTrace, e);
    }

    public static void ErrorWhileProcessingIntegrationEvent(this ILogger logger, string integrationEventId, Exception e)
    {
        ERROR_WHILE_PROCESSING_INTEGRATION_EVENT(logger, integrationEventId, e);
    }
}

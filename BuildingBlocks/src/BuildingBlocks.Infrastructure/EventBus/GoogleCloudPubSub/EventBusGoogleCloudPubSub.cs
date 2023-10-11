using System.Text.RegularExpressions;
using Autofac;
using Azure.Messaging.ServiceBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus.Json;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public class EventBusGoogleCloudPubSub : IEventBus, IDisposable
{
    private static class PubSubMessageAttributes
    {
        public const string EVENT_NAME = "Subject";
    }

    private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";
    private const string AUTOFAC_SCOPE_NAME = "event_bus";

    private readonly ILifetimeScope _autofac;
    private readonly ILogger<EventBusGoogleCloudPubSub> _logger;

    private readonly IGoogleCloudPubSubPersisterConnection _connection;
    private readonly IEventBusSubscriptionsManager _subscriptionManager;
    private readonly HandlerRetryBehavior _handlerRetryBehavior;

    public EventBusGoogleCloudPubSub(IGoogleCloudPubSubPersisterConnection connection,
        ILogger<EventBusGoogleCloudPubSub> logger, IEventBusSubscriptionsManager subscriptionManager,
        ILifetimeScope autofac, HandlerRetryBehavior handlerRetryBehavior)
    {
        _connection = connection;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subscriptionManager = subscriptionManager;
        _autofac = autofac;
        _connection.SubscriberClient.StartAsync(OnIncomingEvent);
        _handlerRetryBehavior = handlerRetryBehavior;
    }

    public void Dispose()
    {
        _subscriptionManager.Clear();
        _connection.Dispose();
    }

    public async void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFFIX, "");

        var jsonMessage = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
        {
            ContractResolver = new ContractResolverWithPrivates()
        });

        var message = new PubsubMessage
        {
            Data = ByteString.CopyFromUtf8(jsonMessage),
            Attributes =
            {
                { PubSubMessageAttributes.EVENT_NAME, eventName }
            }
        };

        var messageId = await _logger.TraceTime(
            () => _connection.PublisherClient.PublishAsync(message), nameof(_connection.PublisherClient.PublishAsync));

        _logger.EventWasNotProcessed(messageId);
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = RemoveIntegrationEventSuffix(typeof(T).Name);

        _logger.LogInformation("Subscribing to event '{EventName}' with {EventHandler}", eventName, typeof(TH).Name);

        _subscriptionManager.AddSubscription<T, TH>();
    }

    private static string RemoveIntegrationEventSuffix(string typeName)
    {
        return Regex.Replace(typeName, $"^(.+){INTEGRATION_EVENT_SUFFIX}$", "$1");
    }

    private async Task<SubscriberClient.Reply> OnIncomingEvent(PubsubMessage @event, CancellationToken _)
    {
        var eventNameFromAttributes =
            $"{@event.Attributes[PubSubMessageAttributes.EVENT_NAME]}{INTEGRATION_EVENT_SUFFIX}";
        var eventData = @event.Data.ToStringUtf8();

        try
        {
            await ProcessEvent(eventNameFromAttributes, eventData);
        }
        catch (Exception ex)
        {
            _logger.ErrorHandlingMessage(ex);
            return SubscriberClient.Reply.Nack;
        }

        // Acknowledge the message so that it is not received again.
        return SubscriberClient.Reply.Ack;
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (!_subscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            _logger.NoSubscriptionForEvent(eventName);
            return;
        }

        await using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);

        var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);
        foreach (var subscription in subscriptions)
        {
            if (scope.ResolveOptional(subscription.HandlerType) is not IIntegrationEventHandler handler)
                throw new Exception(
                    "Integration event handler could not be resolved from dependency container or it does not implement IIntegrationEventHandler.");

            var integrationEvent = (JsonConvert.DeserializeObject(message, subscription.EventType,
                new JsonSerializerSettings
                {
                    ContractResolver = new ContractResolverWithPrivates()
                }) as IntegrationEvent)!;

            var handleMethod = handler.GetType().GetMethod("Handle");

            var policy = EventBusRetryPolicyFactory.Create(
                _handlerRetryBehavior, (ex, _) => _logger.ErrorWhileExecutingEventHandlerType(eventName, ex.Message, ex.StackTrace!, ex));

            await policy.ExecuteAsync(() => (Task)handleMethod!.Invoke(handler, new object[] { integrationEvent })!);
        }
    }
}

file static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception> SENDING_INTEGRATION_EVENT =
        LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(830408, "EventBusGoogleCloudPubSub.SendingIntegrationEvent"),
            "Successfully sent integration event with id '{messageId}'."
        );

    private static readonly Action<ILogger, string, string, Exception> ERROR_HANDLING_MESSAGE =
        LoggerMessage.Define<string, string>(
            LogLevel.Error,
            new EventId(712382, "EventBusGoogleCloudPubSub.ErrorHandlingMessage"),
            "ERROR handling message: '{exceptionMessage}' - Context: '{@exceptionSource}'."
        );

    private static readonly Action<ILogger, string, Exception> NO_SUBSCRIPTION_FOR_EVENT =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(590747, "EventBusGoogleCloudPubSub.NoSubscriptionForEvent"),
            "No subscription for event: '{eventName}'."
        );

    private static readonly Action<ILogger, string, string, string, Exception> ERROR_WHILE_EXECUTING_EVENT_HANDLER_TYPE =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Warning,
            new EventId(304842, "EventBusGoogleCloudPubSub.ErrorWhileExecutingEventHandlerType"),
            "The following error was thrown while executing '{eventHandlerType}':\n'{errorMessage}'\n{stackTrace}.\nAttempting to retry..."
        );

    public static void EventWasNotProcessed(this ILogger logger, string messageId)
    {
        SENDING_INTEGRATION_EVENT(logger, messageId, default!);
    }

    public static void ErrorHandlingMessage(this ILogger logger, Exception e)
    {
        ERROR_HANDLING_MESSAGE(logger, e.Message, e.Source!, e);
    }
    public static void NoSubscriptionForEvent(this ILogger logger, string eventName)
    {
        NO_SUBSCRIPTION_FOR_EVENT(logger, eventName, default!);
    }

    public static void ErrorWhileExecutingEventHandlerType(this ILogger logger, string eventHandlerType, string errorMessage, string stackTrace, Exception e)
    {
        ERROR_WHILE_EXECUTING_EVENT_HANDLER_TYPE(logger, eventHandlerType, errorMessage, stackTrace, e);
    }
}

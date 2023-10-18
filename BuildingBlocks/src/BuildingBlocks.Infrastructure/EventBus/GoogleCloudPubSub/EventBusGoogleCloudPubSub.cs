using System.Text.RegularExpressions;
using Autofac;
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

    public void StartConsuming()
    {
        _connection.SubscriberClient.StartAsync(OnIncomingEvent);
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
            _logger.ErrorHandlingMessage(ex.Message, ex.StackTrace!);
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
                _handlerRetryBehavior, (ex, _) => _logger.ErrorWhileExecutingEventHandlerType(eventName, ex.Message, ex.StackTrace!));

            await policy.ExecuteAsync(() => (Task)handleMethod!.Invoke(handler, new object[] { integrationEvent })!);
        }
    }
}

internal static partial class EventBusGoogleCloudPubSubLogs
{
    [LoggerMessage(
        EventId = 830408,
        EventName = "EventBusGoogleCloudPubSub.SendingIntegrationEvent",
        Level = LogLevel.Debug,
        Message = "Successfully sent integration event with id '{messageId}'.")]
    public static partial void EventWasNotProcessed(this ILogger logger, string messageId);

    [LoggerMessage(
        EventId = 712382,
        EventName = "EventBusGoogleCloudPubSub.ErrorHandlingMessage",
        Level = LogLevel.Error,
        Message = "ERROR handling message: '{exceptionMessage}' - Context: '{exceptionSource}'.")]
    public static partial void ErrorHandlingMessage(this ILogger logger, string exceptionMessage, string exceptionSource);

    [LoggerMessage(
        EventId = 590747,
        EventName = "EventBusGoogleCloudPubSub.NoSubscriptionForEvent",
        Level = LogLevel.Warning,
        Message = "No subscription for event: '{eventName}'.")]
    public static partial void NoSubscriptionForEvent(this ILogger logger, string eventName);

    [LoggerMessage(
        EventId = 304842,
        EventName = "EventBusGoogleCloudPubSub.ErrorWhileExecutingEventHandlerType",
        Level = LogLevel.Warning,
        Message = "The following error was thrown while executing '{eventHandlerType}':\n'{errorMessage}'\n{stackTrace}.\nAttempting to retry...")]
    public static partial void ErrorWhileExecutingEventHandlerType(this ILogger logger, string eventHandlerType, string errorMessage, string stackTrace);
}

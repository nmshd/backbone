﻿using System.Text.RegularExpressions;
using Autofac;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.BuildingBlocks.Infrastructure.EventBus.Json;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

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
            _logger.ErrorHandlingMessage(ex.StackTrace!, ex);
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

        var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);
        foreach (var subscription in subscriptions)
        {
            var integrationEvent = (JsonConvert.DeserializeObject(message, subscription.EventType,
                new JsonSerializerSettings
                {
                    ContractResolver = new ContractResolverWithPrivates()
                }) as IntegrationEvent)!;

            var policy = EventBusRetryPolicyFactory.Create(
                _handlerRetryBehavior, (ex, _) => _logger.ErrorWhileExecutingEventHandlerType(eventName, ex));

            await policy.ExecuteAsync(async () =>
            {
                await using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);

                if (scope.ResolveOptional(subscription.HandlerType) is not IIntegrationEventHandler handler)
                    throw new Exception(
                        "Integration event handler could not be resolved from dependency container or it does not implement IIntegrationEventHandler.");

                var handleMethod = handler.GetType().GetMethod("Handle");

                await (Task)handleMethod!.Invoke(handler, new object[] { integrationEvent })!;

                return Task.CompletedTask;
            });
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
        Message = "Error handling message with context {exceptionSource}.")]
    public static partial void ErrorHandlingMessage(this ILogger logger, string exceptionSource, Exception exception);

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
        Message = "An error was thrown while executing '{eventHandlerType}'. Attempting to retry...")]
    public static partial void ErrorWhileExecutingEventHandlerType(this ILogger logger, string eventHandlerType, Exception exception);
}

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

    private readonly PublisherClient _publisher;

    private readonly IGoogleCloudPubSubPersisterConnection _persisterConnection;
    private readonly IEventBusSubscriptionsManager _subscriptionManager;

    public EventBusGoogleCloudPubSub(IGoogleCloudPubSubPersisterConnection persisterConnection,
        ILogger<EventBusGoogleCloudPubSub> logger, IEventBusSubscriptionsManager subscriptionManager,
        ILifetimeScope autofac)
    {
        _persisterConnection = persisterConnection;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subscriptionManager = subscriptionManager;
        _autofac = autofac;

        _publisher = _persisterConnection.PublisherClient;
    }

    public void Dispose()
    {
        _subscriptionManager.Clear();
        _persisterConnection.Dispose();
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

        var messageId = await _publisher.PublishAsync(message);

        _logger.LogTrace("Successfully sent integration event with id '{messageId}'.", messageId);
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = RemoveIntegrationEventSuffix(typeof(T).Name);

        if (!_subscriptionManager.HasSubscriptionsForEvent<T>())
            RegisterSubscriptionClientMessageHandlerAsync<T>(eventName);

        _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

        _subscriptionManager.AddSubscription<T, TH>();
    }

    private static string RemoveIntegrationEventSuffix(string typeName)
    {
        return Regex.Replace(typeName, $"^(.+){INTEGRATION_EVENT_SUFFIX}$", "$1");
    }

    private void RegisterSubscriptionClientMessageHandlerAsync<T>(string eventName) where T : IntegrationEvent
    {
        var subscriberClient = _persisterConnection.GetSubscriberClient(eventName);
        subscriberClient.StartAsync(OnIncomingEvent<T>); // start listening in the background
    }

    private async Task<SubscriberClient.Reply> OnIncomingEvent<T>(PubsubMessage @event, CancellationToken _)
        where T : IntegrationEvent
    {
        var eventNameFromAttributes =
            $"{@event.Attributes[PubSubMessageAttributes.EVENT_NAME]}{INTEGRATION_EVENT_SUFFIX}";
        var eventData = @event.Data.ToStringUtf8();

        try
        {
            if (!await ProcessEvent<T>(eventNameFromAttributes, eventData))
                _logger.LogInformation($"The event with the MessageId '{@event.MessageId}' wasn't processed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionSource}",
                ex.Message, ex.Source);
            return SubscriberClient.Reply.Nack;
        }

        // Acknowledge the message so that it is not received again.
        return SubscriberClient.Reply.Ack;
    }

    private async Task<bool> ProcessEvent<T>(string eventName, string message) where T : IntegrationEvent
    {
        if (!_subscriptionManager.HasSubscriptionsForEvent(eventName))
            return false;

        await using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);

        var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);
        foreach (var subscription in subscriptions)
        {
            if (scope.ResolveOptional(subscription.HandlerType) is not IIntegrationEventHandler<T> handler)
                throw new Exception(
                    "Integration event handler could not be resolved from dependency container or it does not implement IIntegrationEventHandler.");

            var integrationEvent = JsonConvert.DeserializeObject<T>(message,
                new JsonSerializerSettings
                {
                    ContractResolver = new ContractResolverWithPrivates()
                })!;

            try
            {
                await handler.Handle(integrationEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An error occurred while processing the integration event with id '{eventId}'.",
                    integrationEvent.IntegrationEventId);
                return false;
            }
        }

        return true;
    }
}
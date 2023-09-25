using System.Text;
using Autofac;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

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

        RegisterSubscriptionClientMessageHandlerAsync().GetAwaiter().GetResult();
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

        _logger.LogTrace($"Sending integration event with id '{message.MessageId}'...");

        await _sender.SendMessageAsync(message);

        _logger.LogTrace($"Successfully sent integration event with id '{message.MessageId}'.");
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
                _logger.LogTrace("Trying to create subscription on Service Bus...");

                _serviceBusPersisterConnection.AdministrationClient.CreateRuleAsync(TOPIC_NAME, _subscriptionName,
                    new CreateRuleOptions
                    {
                        Filter = new CorrelationRuleFilter { Subject = eventName },
                        Name = eventName
                    }).GetAwaiter().GetResult();

                _logger.LogTrace("Successfully created subscription on Service Bus.");
            }
            catch (ServiceBusException)
            {
                _logger.LogInformation($"The messaging entity {eventName} already exists.");
            }

        _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

        _subscriptionManager.AddSubscription<T, TH>();
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
                    _logger.LogInformation(
                        $"The event with the MessageId '{args.Message.MessageId}' wasn't processed and will therefore not be completed.");
            };

        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        var ex = args.Exception;
        var context = args.ErrorSource;

        _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}",
            ex.Message, context);

        return Task.CompletedTask;
    }

    private async Task<bool> ProcessEvent(string eventName, string message)
    {
        if (!_subscriptionManager.HasSubscriptionsForEvent(eventName))
            return false;

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
                var policy = Policy.Handle<Exception>()
                    .WaitAndRetryAsync(
                        _handlerRetryBehavior.NumberOfRetries,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(_handlerRetryBehavior.MinimumBackoff, retryAttempt)),
                        (ex, _) => _logger.LogWarning(ex.ToString()))
                    .WrapAsync(Policy.TimeoutAsync(_handlerRetryBehavior.MaximumBackoff));

                await policy.ExecuteAsync(() => (Task)concreteType.GetMethod("Handle")!.Invoke(handler, new[] { integrationEvent })!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"An error occurred while processing the integration event with id '{integrationEvent.IntegrationEventId}'.");
                return false;
            }
        }


        return true;
    }
}

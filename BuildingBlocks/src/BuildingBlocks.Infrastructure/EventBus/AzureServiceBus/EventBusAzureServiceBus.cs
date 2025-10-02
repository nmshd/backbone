using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public class EventBusAzureServiceBus : IEventBus, IDisposable, IAsyncDisposable
{
    private const string TOPIC_NAME = "default";
    private const int MAX_DELIVERY_COUNT = 5;

    private readonly ServiceBusProcessorOptions _options = new()
    {
        AutoCompleteMessages = false,
        MaxConcurrentCalls = 1,
        PrefetchCount = 10
    };

    private readonly IServiceProvider _serviceProvider;
    private readonly EventBusMetrics _metrics;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusAdministrationClient _adminClient;
    private readonly ServiceBusSender _sender;
    private readonly ILogger<EventBusAzureServiceBus> _logger;

    private readonly List<ServiceBusProcessor> _processors = [];

    public EventBusAzureServiceBus(ServiceBusClient client, ServiceBusAdministrationClient adminClient, ILogger<EventBusAzureServiceBus> logger, IServiceProvider serviceProvider,
        EventBusMetrics metrics)
    {
        _client = client;
        _adminClient = adminClient;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider;
        _metrics = metrics;
        _sender = client.CreateSender(TOPIC_NAME);
    }

    public void Dispose()
    {
        Task.Run(async () => await DisposeAsync()).GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var processor in _processors)
        {
            await processor.CloseAsync();
        }
    }

    public async Task Publish(DomainEvent @event)
    {
        var eventName = @event.GetEventName();
        var jsonMessage = JsonSerializer.Serialize(@event, @event.GetType());
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        _metrics.TrackHandledMessageSize(body.Length);

        var message = new ServiceBusMessage
        {
            MessageId = @event.DomainEventId,
            Body = new BinaryData(body),
            Subject = eventName,
            CorrelationId = CustomLogContext.GetCorrelationId()
        };

        _logger.SendingDomainEvent(message.MessageId);

        try
        {
            var startedAt = Stopwatch.GetTimestamp();
            await _sender.SendMessageAsync(message);
            _metrics.TrackEventPublishingDuration(startedAt);

            _metrics.IncrementNumberOfPublishedEvents(eventName);

            _logger.LogDebug("Successfully sent domain event with id '{MessageId}'.", message.MessageId);
        }
        catch (Exception)
        {
            _metrics.IncrementNumberOfPublishingErrors(eventName);
            throw;
        }
    }

    public async Task Subscribe<T, TH>()
        where T : DomainEvent
        where TH : IDomainEventHandler<T>
    {
        var eventName = typeof(T).GetEventName();
        var subscriptionName = GetSubscriptionName<TH, T>();

        await EnsureSubscriptionExists(subscriptionName);

        await RegisterSubscriptionForEvent(subscriptionName, eventName);

        var processor = _client.CreateProcessor(TOPIC_NAME, subscriptionName, _options);

        processor.ProcessMessageAsync +=
            async args =>
            {
                var messageData = args.Message.Body.ToString();
                var correlationId = args.Message.CorrelationId;

                correlationId = correlationId.IsNullOrEmpty() ? CustomLogContext.GenerateCorrelationId() : correlationId;

                using (CustomLogContext.SetCorrelationId(correlationId))
                {
                    var processedSuccessfully = await ProcessEvent<T, TH>(messageData);

                    if (processedSuccessfully)
                        await args.CompleteMessageAsync(args.Message);
                    else
                    {
                        await args.AbandonMessageAsync(args.Message);
                        _metrics.IncrementNumberOfProcessingErrors(GetSubscriptionName<TH, T>());
                        _logger.EventWasNotProcessed(args.Message.MessageId);
                    }
                }
            };

        processor.ProcessErrorAsync += ErrorHandler;

        _processors.Add(processor);
    }

    public bool IsConnected => !_client.IsClosed;

    private async Task EnsureSubscriptionExists(string subscriptionName)
    {
        if (!await _adminClient.SubscriptionExistsAsync(TOPIC_NAME, subscriptionName))
        {
            _logger.LogInformation("Creating subscription on Service Bus...");

            await _adminClient.CreateSubscriptionAsync(new CreateSubscriptionOptions(TOPIC_NAME, subscriptionName)
            {
                MaxDeliveryCount = MAX_DELIVERY_COUNT,
                DeadLetteringOnMessageExpiration = true,
            });

            _logger.LogInformation("Successfully created subscription on Service Bus.");
        }

        if (await _adminClient.RuleExistsAsync(TOPIC_NAME, subscriptionName, "$Default"))
        {
            await _adminClient.DeleteRuleAsync(TOPIC_NAME, subscriptionName, "$Default");
        }
    }

    private async Task RegisterSubscriptionForEvent(string subscriptionName, string eventName)
    {
        if (!await _adminClient.RuleExistsAsync(TOPIC_NAME, subscriptionName, eventName))
        {
            _logger.LogInformation("Creating rule on subscription...");

            await _adminClient.CreateRuleAsync(TOPIC_NAME, subscriptionName,
                new CreateRuleOptions
                {
                    Filter = new CorrelationRuleFilter { Subject = eventName },
                    Name = eventName
                });

            _logger.LogInformation("Successfully created rule on subscription.");
        }
    }

    public async Task StartConsuming(CancellationToken cancellationToken)
    {
        await RegisterSubscriptionClientMessageHandler(cancellationToken);
    }

    private async Task RegisterSubscriptionClientMessageHandler(CancellationToken cancellationToken)
    {
        foreach (var processor in _processors)
        {
            await processor.StartProcessingAsync(cancellationToken);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        var ex = args.Exception;
        var context = args.ErrorSource;

        _logger.ErrorHandlingMessage(context, ex);

        return Task.CompletedTask;
    }

    private async Task<bool> ProcessEvent<TEvent, THandler>(string message) where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);

        var domainEvent = JsonSerializer.Deserialize<TEvent>(message)!;
        var concreteType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            if (scope.ServiceProvider.GetService(typeof(THandler)) is not IDomainEventHandler handler)
                throw new Exception($"Domain event handler '{typeof(THandler).FullName}' could not be resolved from dependency container or it does not implement {nameof(IDomainEventHandler)}.");

            var startedAt = Stopwatch.GetTimestamp();
            await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, [domainEvent])!;
            _metrics.TrackEventProcessingDuration(startedAt, GetSubscriptionName<THandler, TEvent>());

            _metrics.IncrementNumberOfHandledEvents(GetSubscriptionName<THandler, TEvent>());
        }
        catch (Exception ex)
        {
            _logger.ErrorWhileProcessingDomainEvent(domainEvent.DomainEventId, ex);
            return false;
        }

        return true;
    }

    public async Task StopConsuming(CancellationToken cancellationToken)
    {
        foreach (var processor in _processors)
        {
            await processor.StopProcessingAsync(cancellationToken);
        }
    }

    public static string GetSubscriptionName<THandler, TEvent>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventHandlerFullName = typeof(THandler).FullName!;

        var moduleName = eventHandlerFullName.Split('.').ElementAt(2);

        return $"{moduleName}.{typeof(TEvent).GetEventName()}".TruncateToXChars(50);
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
        EventId = 146670,
        EventName = "EventBusAzureServiceBus.ErrorWhileProcessingDomainEvent",
        Level = LogLevel.Error,
        Message = "An error occurred while processing the event with id '{domainEventId}'.")]
    public static partial void ErrorWhileProcessingDomainEvent(this ILogger logger, string domainEventId, Exception ex);
}

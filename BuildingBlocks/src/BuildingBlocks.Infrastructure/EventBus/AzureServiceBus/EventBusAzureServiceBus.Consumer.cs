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
using OpenTelemetry;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public partial class EventBusAzureServiceBus
{
    private const string PROCESS_OPERATION_NAME = "consume";
    private const string PROCESS_OPERATION_TYPE = "process";

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
                using var activity = StartProcessActivity(args.Message, subscriptionName, args.Message.Body.ToMemory().Length);

                var messageData = args.Message.Body.ToString();

                activity?.AddEvent(new ActivityEvent("enmeshed.backbone.event_bus.consumer.message_decoded"));

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
        Activity.Current?.AddException(args.Exception);

        return Task.CompletedTask;
    }

    private async Task<bool> ProcessEvent<TEvent, THandler>(string message) where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        Activity.Current?.AddEvent(new ActivityEvent("enmeshed.backbone.event_bus.consumer.start_processing"));

        var eventType = typeof(TEvent);

        var domainEvent = JsonSerializer.Deserialize<TEvent>(message)!;
        var concreteType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            if (scope.ServiceProvider.GetService(typeof(THandler)) is not IDomainEventHandler handler)
                throw new Exception($"Domain event handler '{typeof(THandler).FullName}' could not be resolved from dependency container or it does not implement {nameof(IDomainEventHandler)}.");

            Activity.Current?.AddEvent(new ActivityEvent("enmeshed.backbone.event_bus.consumer.handler_resolved"));

            var startedAt = Stopwatch.GetTimestamp();
            await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, [domainEvent])!;
            _metrics.TrackEventProcessingDuration(startedAt, GetSubscriptionName<THandler, TEvent>());

            _metrics.IncrementNumberOfHandledEvents(GetSubscriptionName<THandler, TEvent>());
        }
        catch (Exception ex)
        {
            Activity.Current?.AddException(ex);
            return false;
        }

        return true;
    }

    private Activity? StartProcessActivity(ServiceBusReceivedMessage message, string subscriptionName, int bodySize)
    {
        var parentContext = EventBusDiagnostics.PROPAGATOR.Extract(default,
            message.ApplicationProperties,
            ExtractTraceContextFromApplicationProperties);
        Baggage.Current = parentContext.Baggage;

        var destinationName = $"{TOPIC_NAME}.{subscriptionName}";
        var activity = EventBusDiagnostics.ACTIVITY_SOURCE.StartActivity($"{PROCESS_OPERATION_NAME} {destinationName}", ActivityKind.Consumer, parentContext.ActivityContext);

        if (activity == null)
            return null;

        activity.SetTag("messaging.system", MESSAGING_SYSTEM);
        activity.SetTag("messaging.operation.name", PROCESS_OPERATION_NAME);
        activity.SetTag("messaging.operation.type", PROCESS_OPERATION_TYPE);
        activity.SetTag("messaging.destination.name", destinationName);
        activity.SetTag("messaging.destination.subscription.name", subscriptionName);
        activity.SetTag("messaging.destination.template", $"{TOPIC_NAME}:{{subscriptionName}}");
        activity.SetTag("messaging.message.body.size", bodySize);
        activity.SetTag("messaging.servicebus.message.delivery_count", message.DeliveryCount);
        activity.SetTag("messaging.servicebus.message.enqueued_time", message.EnqueuedTime.ToUnixTimeSeconds());

        if (!message.MessageId.IsNullOrEmpty())
            activity.SetTag("messaging.message.id", message.MessageId);

        if (!message.CorrelationId.IsNullOrEmpty())
            activity.SetTag("messaging.message.conversation_id", message.CorrelationId);

        return activity;
    }

    private IEnumerable<string> ExtractTraceContextFromApplicationProperties(IReadOnlyDictionary<string, object> applicationProperties, string key)
    {
        if (!applicationProperties.TryGetValue(key, out var value)) return [];

        if (value is string stringValue) return [stringValue];
        if (value is byte[] bytes) return [Encoding.UTF8.GetString(bytes)];

        return [];
    }

    public async Task StopConsuming(CancellationToken cancellationToken)
    {
        foreach (var processor in _processors)
        {
            await processor.StopProcessingAsync(cancellationToken);
        }
    }
}

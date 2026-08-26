using System.Diagnostics;
using System.Text.Json;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.Tooling.Extensions;
using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using Type = System.Type;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public partial class EventBusGoogleCloudPubSub
{
    private const string PROCESS_OPERATION_NAME = "consume";
    private const string PROCESS_OPERATION_TYPE = "process";

    public async Task Subscribe<T, TH>()
        where T : DomainEvent
        where TH : IDomainEventHandler<T>
    {
        var eventName = typeof(T).GetEventName();
        var subscriptionName = GetSubscriptionName<TH, T>(_projectId);

        await EnsureSubscriptionExists(subscriptionName, eventName);

        var subscriberClient = await new SubscriberClientBuilder
        {
            SubscriptionName = subscriptionName,
            GoogleCredential = _gcpCredentials,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
        }.BuildAsync();

        _subscriptions.Add(new Subscription(subscriberClient, typeof(T), typeof(TH)));
    }

    private async Task EnsureSubscriptionExists(SubscriptionName subscriptionName, string eventName)
    {
        try
        {
            var subscriptionRequest = new Google.Cloud.PubSub.V1.Subscription
            {
                SubscriptionName = subscriptionName,
                TopicAsTopicName = _topicName,
                Filter = $"attributes.{PubSubMessageAttributes.EVENT_NAME} = \"{eventName}\"",
                AckDeadlineSeconds = (int)MESSAGE_ACK_DEADLINE.TotalSeconds,
                RetryPolicy = new RetryPolicy
                {
                    MinimumBackoff = Duration.FromTimeSpan(SUBSCRIPTION_MINIMUM_BACKOFF.Seconds()),
                    MaximumBackoff = Duration.FromTimeSpan(SUBSCRIPTION_MAXIMUM_BACKOFF.Seconds())
                }
            };

            _logger.LogInformation("Creating subscription '{SubscriptionName}' for event '{EventName}'...", subscriptionName, eventName);

            await _subscriberService.CreateSubscriptionAsync(subscriptionRequest);

            _logger.LogInformation("Successfully created subscription '{SubscriptionName}' for event '{EventName}'.", subscriptionName, eventName);
        }
        catch (RpcException ex)
        {
            if (ex.StatusCode == StatusCode.AlreadyExists)
            {
                _logger.LogInformation("Subscription '{SubscriptionName}' for event '{EventName}' already exists.", subscriptionName, eventName);
                return;
            }

            throw;
        }
    }

    public async Task StartConsuming(CancellationToken cancellationToken)
    {
        var consumptionTasks = _subscriptions.Select(s => s.SubscriberClient.StartAsync((e, _) => OnIncomingEvent(e, s.EventType, s.HandlerType)));

        await Task.WhenAll(consumptionTasks);
    }

    private async Task<SubscriberClient.Reply> OnIncomingEvent(PubsubMessage @event, Type eventType, Type handlerType)
    {
        var subscriptionName = GetSubscriptionName(_projectId, handlerType, eventType).SubscriptionId;

        using var activity = StartProcessActivity(@event, subscriptionName, @event.Data.Length);

        var eventData = @event.Data.ToStringUtf8();

        activity?.AddEvent(new ActivityEvent("enmeshed.backbone.event_bus.consumer.message_decoded"));

        try
        {
            @event.Attributes.TryGetValue(PubSubMessageAttributes.CORRELATION_ID, out var correlationId);

            correlationId = correlationId.IsNullOrEmpty() ? CustomLogContext.GenerateCorrelationId() : correlationId;

            using (CustomLogContext.SetCorrelationId(correlationId))
            {
                await ProcessEvent(eventData, eventType, handlerType);
            }
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);

            _metrics.IncrementNumberOfProcessingErrors(subscriptionName);
            activity?.AddException(ex);
            return SubscriberClient.Reply.Nack;
        }

        return SubscriberClient.Reply.Ack;
    }

    private async Task ProcessEvent(string message, Type eventType, Type handlerType)
    {
        Activity.Current?.AddEvent(new ActivityEvent("enmeshed.backbone.event_bus.consumer.start_processing"));

        var subscriptionName = GetSubscriptionName(_projectId, handlerType, eventType).SubscriptionId;
        var domainEvent = JsonSerializer.Deserialize(message, eventType)!;

        await using var scope = _serviceProvider.CreateAsyncScope();

        if (scope.ServiceProvider.GetService(handlerType) is not IDomainEventHandler handler)
            throw new Exception("Domain event handler could not be resolved from dependency container or it does not implement IDomainEventHandler.");

        Activity.Current?.AddEvent(new ActivityEvent("enmeshed.backbone.event_bus.consumer.handler_resolved"));

        var handleMethod = handler.GetType().GetMethod("Handle");

        var startedAt = Stopwatch.GetTimestamp();
        await (Task)handleMethod!.Invoke(handler, [domainEvent])!;
        _metrics.TrackEventProcessingDuration(startedAt, subscriptionName);

        _metrics.IncrementNumberOfHandledEvents(subscriptionName);
    }

    private Activity? StartProcessActivity(PubsubMessage message, string subscriptionName, int bodySize)
    {
        var parentContext = EventBusDiagnostics.PROPAGATOR.Extract(default,
            message.Attributes,
            ExtractTraceContextFromAttributes);
        Baggage.Current = parentContext.Baggage;

        var destinationName = $"{_topicName.TopicId}.{subscriptionName}";
        var activity = EventBusDiagnostics.ACTIVITY_SOURCE.StartActivity($"{PROCESS_OPERATION_NAME} {destinationName}", ActivityKind.Consumer, parentContext.ActivityContext);

        if (activity == null)
            return null;

        activity.SetTag("messaging.system", MESSAGING_SYSTEM);
        activity.SetTag("messaging.operation.name", PROCESS_OPERATION_NAME);
        activity.SetTag("messaging.operation.type", PROCESS_OPERATION_TYPE);
        activity.SetTag("messaging.destination.name", destinationName);
        activity.SetTag("messaging.destination.subscription.name", subscriptionName);
        activity.SetTag("messaging.destination.template", $"{_topicName.TopicId}:{{subscriptionName}}");
        activity.SetTag("messaging.message.body.size", bodySize);

        if (!message.MessageId.IsNullOrEmpty())
            activity.SetTag("messaging.message.id", message.MessageId);

        if (!message.OrderingKey.IsNullOrEmpty())
            activity.SetTag("messaging.gcp_pubsub.message.ordering_key", message.OrderingKey);

        if (message.Attributes.TryGetValue(PubSubMessageAttributes.CORRELATION_ID, out var correlationId) && !correlationId.IsNullOrEmpty())
            activity.SetTag("messaging.message.conversation_id", correlationId);

        return activity;
    }

    private IEnumerable<string> ExtractTraceContextFromAttributes(IDictionary<string, string> attributes, string key)
    {
        if (!attributes.TryGetValue(key, out var value)) return [];

        return [value];
    }

    public async Task StopConsuming(CancellationToken cancellationToken)
    {
        var stopTasks = _subscriptions.Select(subscription => subscription.SubscriberClient.StopAsync(new SubscriberClient.ShutdownOptions(), CancellationToken.None));

        await Task.WhenAll(stopTasks);
    }
}

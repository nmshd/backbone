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
using Type = System.Type;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public partial class EventBusGoogleCloudPubSub
{
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
        var eventData = @event.Data.ToStringUtf8();

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
            _metrics.IncrementNumberOfProcessingErrors(GetSubscriptionName(_projectId, handlerType, eventType).SubscriptionId);
            _logger.ErrorHandlingMessage(ex.StackTrace!, ex);
            return SubscriberClient.Reply.Nack;
        }

        return SubscriberClient.Reply.Ack;
    }

    private async Task ProcessEvent(string message, Type eventType, Type handlerType)
    {
        var subscriptionName = GetSubscriptionName(_projectId, handlerType, eventType).SubscriptionId;
        var domainEvent = JsonSerializer.Deserialize(message, eventType)!;

        await using var scope = _serviceProvider.CreateAsyncScope();

        if (scope.ServiceProvider.GetService(handlerType) is not IDomainEventHandler handler)
            throw new Exception("Domain event handler could not be resolved from dependency container or it does not implement IDomainEventHandler.");

        var handleMethod = handler.GetType().GetMethod("Handle");

        var startedAt = Stopwatch.GetTimestamp();
        await (Task)handleMethod!.Invoke(handler, [domainEvent])!;
        _metrics.TrackEventProcessingDuration(startedAt, subscriptionName);

        _metrics.IncrementNumberOfHandledEvents(subscriptionName);
    }

    public async Task StopConsuming(CancellationToken cancellationToken)
    {
        var stopTasks = _subscriptions.Select(subscription => subscription.SubscriberClient.StopAsync(new SubscriberClient.ShutdownOptions(), CancellationToken.None));

        await Task.WhenAll(stopTasks);
    }
}

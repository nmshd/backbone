using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Polly.Retry;
using RabbitMQ.Client;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public partial class EventBusRabbitMq
{
    private const int PUBLISH_RETRY_COUNT = 6;
    private const string PUBLISH_OPERATION_NAME = "publish";
    private const string SEND_OPERATION_TYPE = "send";

    private readonly AsyncRetryPolicy _publishRetryPolicy;

    public async Task Publish(DomainEvent @event)
    {
        var eventName = @event.GetEventName();
        var message = JsonSerializer.Serialize(@event, @event.GetType());
        var body = Encoding.UTF8.GetBytes(message);

        _metrics.TrackHandledMessageSize(body.Length);

        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            MessageId = @event.DomainEventId,
            CorrelationId = CustomLogContext.GetCorrelationId()
        };
        using var activity = StartPublishActivity(properties, @event.DomainEventId, eventName, body.Length);

        await _publishRetryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var channel = await _channelPool.Get();

                activity?.AddEvent(new ActivityEvent("enmeshed.backbone.event_bus.publisher.created_channel_for_publish"));

                var startedAt = Stopwatch.GetTimestamp();
                await channel.BasicPublishAsync(_exchangeName, eventName, mandatory: false, properties, body);

                _metrics.TrackEventPublishingDuration(startedAt);
                _metrics.IncrementNumberOfPublishedEvents(eventName);

                _channelPool.Return(channel);
            }
            catch (Exception ex)
            {
                _metrics.IncrementNumberOfPublishingErrors(eventName);
                Activity.Current?.AddException(ex);
                throw;
            }
        });
    }

    private Activity? StartPublishActivity(BasicProperties properties, string messageId, string eventName, int bodySize)
    {
        var destinationName = $"{_exchangeName}:{eventName}";

        var activity = EventBusDiagnostics.ACTIVITY_SOURCE.StartActivity($"{PUBLISH_OPERATION_NAME} {destinationName}", ActivityKind.Producer, Activity.Current?.Context ?? default);

        ActivityContext contextToInject = default;

        if (activity != null)
            contextToInject = activity.Context;
        else if (Activity.Current != null)
            contextToInject = Activity.Current.Context;

        EventBusDiagnostics.PROPAGATOR.Inject(new PropagationContext(contextToInject, Baggage.Current), properties, InjectTraceContextIntoProperties);

        if (activity == null)
            return null;

        activity.SetTag("messaging.system", MESSAGING_SYSTEM);
        activity.SetTag("messaging.operation.name", PUBLISH_OPERATION_NAME);
        activity.SetTag("messaging.operation.type", SEND_OPERATION_TYPE);
        activity.SetTag("messaging.destination.name", destinationName);
        activity.SetTag("messaging.message.id", messageId);
        activity.SetTag("messaging.message.body.size", bodySize);
        activity.SetTag("messaging.destination.template", $"{_exchangeName}:{{eventName}}");
        activity.SetTag("messaging.rabbitmq.destination.routing_key", eventName);

        return activity;
    }

    private void InjectTraceContextIntoProperties(IBasicProperties properties, string key, string value)
    {
        properties.Headers ??= new Dictionary<string, object?>();
        properties.Headers[key] = value;
    }
}

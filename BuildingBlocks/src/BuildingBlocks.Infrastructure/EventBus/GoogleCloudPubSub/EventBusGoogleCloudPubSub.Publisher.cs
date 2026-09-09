using System.Diagnostics;
using System.Text.Json;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.Tooling.Extensions;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public partial class EventBusGoogleCloudPubSub
{
    private const string PUBLISH_OPERATION_NAME = "publish";
    private const string SEND_OPERATION_TYPE = "send";

    public async Task Publish(DomainEvent @event)
    {
        var eventName = @event.GetEventName();
        var jsonMessage = JsonSerializer.Serialize(@event, @event.GetType());
        var messageBytes = ByteString.CopyFromUtf8(jsonMessage);

        _metrics.TrackHandledMessageSize(messageBytes.Length);

        var message = new PubsubMessage
        {
            Data = messageBytes,
            Attributes =
            {
                { PubSubMessageAttributes.EVENT_NAME, eventName },
                { PubSubMessageAttributes.CORRELATION_ID, CustomLogContext.GetCorrelationId() }
            }
        };

        using var activity = StartPublishActivity(message, eventName, messageBytes.Length);

        try
        {
            var startedAt = Stopwatch.GetTimestamp();
            var messageId = await _publisherClient.PublishAsync(message);
            activity?.SetTag("messaging.message.id", messageId);

            _metrics.TrackEventPublishingDuration(startedAt);
            _metrics.IncrementNumberOfPublishedEvents(eventName);
        }
        catch (Exception ex)
        {
            _metrics.IncrementNumberOfPublishingErrors(eventName);
            activity?.AddException(ex);
            throw;
        }
    }

    private Activity? StartPublishActivity(PubsubMessage message, string eventName, int bodySize)
    {
        var destinationName = $"{_topicName.TopicId}:{eventName}";

        var activity = EventBusDiagnostics.ACTIVITY_SOURCE.StartActivity($"{PUBLISH_OPERATION_NAME} {destinationName}", ActivityKind.Producer, Activity.Current?.Context ?? default);

        ActivityContext contextToInject = default;

        if (activity != null)
            contextToInject = activity.Context;
        else if (Activity.Current != null)
            contextToInject = Activity.Current.Context;

        EventBusDiagnostics.PROPAGATOR.Inject(new PropagationContext(contextToInject, Baggage.Current), message.Attributes, InjectTraceContextIntoAttributes);

        if (activity == null)
            return null;

        activity.SetTag("messaging.system", MESSAGING_SYSTEM);
        activity.SetTag("messaging.operation.name", PUBLISH_OPERATION_NAME);
        activity.SetTag("messaging.operation.type", SEND_OPERATION_TYPE);
        activity.SetTag("messaging.destination.name", destinationName);
        activity.SetTag("messaging.message.body.size", bodySize);
        activity.SetTag("messaging.destination.template", $"{_topicName.TopicId}:{{eventName}}");

        if (message.Attributes.TryGetValue(PubSubMessageAttributes.CORRELATION_ID, out var correlationId) && !correlationId.IsNullOrEmpty())
            activity.SetTag("messaging.message.conversation_id", correlationId);

        return activity;
    }

    private void InjectTraceContextIntoAttributes(IDictionary<string, string> attributes, string key, string value)
    {
        attributes[key] = value;
    }
}

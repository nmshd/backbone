using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public partial class EventBusAzureServiceBus
{
    private const string PUBLISH_OPERATION_NAME = "publish";
    private const string SEND_OPERATION_TYPE = "send";

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

        using var activity = StartPublishActivity(message, @event.DomainEventId, eventName, body.Length);

        _logger.SendingDomainEvent(message.MessageId);

        try
        {
            var startedAt = Stopwatch.GetTimestamp();
            await _sender.SendMessageAsync(message);
            _metrics.TrackEventPublishingDuration(startedAt);

            _metrics.IncrementNumberOfPublishedEvents(eventName);

            _logger.LogDebug("Successfully sent domain event with id '{MessageId}'.", message.MessageId);
        }
        catch (Exception ex)
        {
            _metrics.IncrementNumberOfPublishingErrors(eventName);
            activity?.AddException(ex);
            throw;
        }
    }

    private Activity? StartPublishActivity(ServiceBusMessage message, string messageId, string eventName, int bodySize)
    {
        var destinationName = $"{TOPIC_NAME}:{eventName}";

        var activity = EventBusDiagnostics.ACTIVITY_SOURCE.StartActivity($"{PUBLISH_OPERATION_NAME} {destinationName}", ActivityKind.Producer, Activity.Current?.Context ?? default);

        ActivityContext contextToInject = default;

        if (activity != null)
            contextToInject = activity.Context;
        else if (Activity.Current != null)
            contextToInject = Activity.Current.Context;

        EventBusDiagnostics.PROPAGATOR.Inject(new PropagationContext(contextToInject, Baggage.Current), message.ApplicationProperties, InjectTraceContextIntoApplicationProperties);

        if (activity == null)
            return null;

        activity.SetTag("messaging.system", MESSAGING_SYSTEM);
        activity.SetTag("messaging.operation.name", PUBLISH_OPERATION_NAME);
        activity.SetTag("messaging.operation.type", SEND_OPERATION_TYPE);
        activity.SetTag("messaging.destination.name", destinationName);
        activity.SetTag("messaging.message.id", messageId);
        activity.SetTag("messaging.message.body.size", bodySize);
        activity.SetTag("messaging.destination.template", $"{TOPIC_NAME}:{{eventName}}");

        if (!message.CorrelationId.IsNullOrEmpty())
            activity.SetTag("messaging.message.conversation_id", message.CorrelationId);

        return activity;
    }

    private void InjectTraceContextIntoApplicationProperties(IDictionary<string, object> applicationProperties, string key, string value)
    {
        applicationProperties[key] = value;
    }
}

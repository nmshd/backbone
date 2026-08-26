using System.Diagnostics;
using System.Text.Json;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public partial class EventBusGoogleCloudPubSub
{
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

        try
        {
            var startedAt = Stopwatch.GetTimestamp();
            var messageId = await _publisherClient.PublishAsync(message);
            _logger.SuccessfullySentDomainEvent(messageId);

            _metrics.TrackEventPublishingDuration(startedAt);
            _metrics.IncrementNumberOfPublishedEvents(eventName);
        }
        catch (Exception)
        {
            _metrics.IncrementNumberOfPublishingErrors(eventName);
            throw;
        }
    }
}

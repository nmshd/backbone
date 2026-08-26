using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public partial class EventBusAzureServiceBus
{
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
}

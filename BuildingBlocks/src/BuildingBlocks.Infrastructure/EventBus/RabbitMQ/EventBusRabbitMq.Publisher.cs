using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public partial class EventBusRabbitMq
{
    public async Task Publish(DomainEvent @event)
    {
        var eventName = @event.GetEventName();

        _logger.LogInformation("Creating RabbitMQ channel to publish a '{EventName}'.", eventName);

        var message = JsonSerializer.Serialize(@event, @event.GetType());

        var body = Encoding.UTF8.GetBytes(message);

        _metrics.TrackHandledMessageSize(body.Length);

        await _publishRetryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogDebug("Publishing a '{EventName}' event to RabbitMQ.", eventName);

            var channel = await _channelPool.Get();
            var properties = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = @event.DomainEventId,
                CorrelationId = CustomLogContext.GetCorrelationId()
            };

            try
            {
                var startedAt = Stopwatch.GetTimestamp();
                await channel.BasicPublishAsync(_exchangeName, eventName, mandatory: false, properties, body);
                _logger.PublishedDomainEvent();

                _metrics.TrackEventPublishingDuration(startedAt);
                _metrics.IncrementNumberOfPublishedEvents(eventName);
            }
            catch (Exception)
            {
                _metrics.IncrementNumberOfPublishingErrors(eventName);
                throw;
            }

            _channelPool.Return(channel);
        });
    }
}

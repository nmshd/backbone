using System.Diagnostics;
using System.Diagnostics.Metrics;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public class EventBusMetrics
{
    private readonly UpDownCounter<int> _numberOfActiveHandlers;
    private readonly Counter<long> _numberOfHandledEvents;
    private readonly Histogram<double> _eventProcessingDuration;
    private readonly Counter<long> _numberOfProcessingErrors;

    private readonly Histogram<double> _eventPublishingDuration;
    private readonly Counter<long> _numberOfPublishedEvents;
    private readonly Histogram<int> _messageSizeBytes;
    private readonly Counter<long> _numberOfPublishingErrors;

    public EventBusMetrics(Meter meter)
    {
        _numberOfHandledEvents = meter.CreateCounter<long>(name: "enmeshed_events_handled_total");
        _eventProcessingDuration = meter.CreateHistogram(name: "enmeshed_events_processing_duration_seconds", unit: "s", advice: new InstrumentAdvice<double>
        {
            HistogramBucketBoundaries = [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10, 30, 60, 180, 600]
        });
        _numberOfActiveHandlers = meter.CreateUpDownCounter<int>(name: "enmeshed_events_active_handlers");
        _numberOfProcessingErrors = meter.CreateCounter<long>(name: "enmeshed_events_processing_errors_total");

        _numberOfPublishedEvents = meter.CreateCounter<long>(name: "enmeshed_events_published_total");
        _eventPublishingDuration = meter.CreateHistogram(name: "enmeshed_events_publishing_duration_seconds", unit: "s", advice: new InstrumentAdvice<double>
        {
            HistogramBucketBoundaries = [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10, 30, 60, 180, 600]
        });
        _messageSizeBytes = meter.CreateHistogram<int>(name: "enmeshed_events_message_size_bytes", unit: "By");
        _numberOfPublishingErrors = meter.CreateCounter<long>(name: "enmeshed_events_publishing_errors_total");
    }

    #region processing

    public void TrackEventProcessingDuration(EventProcessingDurationStage stage, long startedAt, string eventName, string queueName)
    {
        _eventProcessingDuration.Record(
            Stopwatch.GetElapsedTime(startedAt).TotalSeconds,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("queue_name", queueName),
            new KeyValuePair<string, object?>("stage", EventProcessingDurationStageToString(stage)));
    }

    public void IncrementNumberOfHandledEvents(string eventName, string queueName)
    {
        _numberOfHandledEvents.Add(
            1,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("queue_name", queueName)
        );
    }

    public void IncrementNumberOfActiveHandlers(string eventName, string queueName)
    {
        _numberOfActiveHandlers.Add(
            1,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("queue_name", queueName)
        );
    }

    public void DecrementNumberOfActiveHandlers(string eventName, string queueName)
    {
        _numberOfActiveHandlers.Add(
            -1,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("queue_name", queueName)
        );
    }

    private string EventProcessingDurationStageToString(EventProcessingDurationStage stage)
    {
        return stage switch
        {
            EventProcessingDurationStage.Deserialize => "deserialize",
            EventProcessingDurationStage.Handle => "handle",
            EventProcessingDurationStage.Reject => "reject",
            EventProcessingDurationStage.Acknowledge => "acknowledge",
            _ => "unknown"
        };
    }

    public void IncrementNumberOfProcessingErrors(string eventName, string queueName)
    {
        _numberOfProcessingErrors.Add(
            1,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("queue_name", queueName)
        );
    }

    #endregion

    #region publishing

    public void TrackEventPublishingDuration(EventPublishingDurationStage stage, long startedAt, string eventName)
    {
        _eventPublishingDuration.Record(
            Stopwatch.GetElapsedTime(startedAt).TotalSeconds,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("stage", EventPublishingDurationStageToString(stage)));
    }

    public void IncrementNumberOfPublishedEvents(string eventName, string queueName)
    {
        _numberOfPublishedEvents.Add(
            1,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("queue_name", queueName)
        );
    }

    public void TrackHandledMessageSize(int messageSize, string eventName)
    {
        _messageSizeBytes.Record(messageSize, new KeyValuePair<string, object?>("event_name", eventName));
    }

    private string EventPublishingDurationStageToString(EventPublishingDurationStage stage)
    {
        return stage switch
        {
            EventPublishingDurationStage.Publish => "publish",
            EventPublishingDurationStage.Serialize => "serialize",
            _ => "unknown"
        };
    }

    public void IncrementNumberOfPublishingErrors(string eventName)
    {
        _numberOfPublishingErrors.Add(
            1,
            new KeyValuePair<string, object?>("event_name", eventName)
        );
    }

    #endregion
}

using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public class EventBusMetrics
{
    private readonly Counter<long> _numberOfProcessedEvents;
    private readonly Histogram<double> _eventProcessingDuration;
    private readonly Counter<long> _numberOfProcessingErrors;

    private readonly Histogram<double> _eventPublishingDuration;
    private readonly Counter<long> _numberOfPublishedEvents;
    private readonly Histogram<int> _messageSizeBytes;
    private readonly Counter<long> _numberOfPublishingErrors;

    public EventBusMetrics(Meter meter)
    {
        _numberOfProcessedEvents = meter.CreateCounter<long>(name: "enmeshed_events_handled_total");
        _eventProcessingDuration = meter.CreateHistogram(name: "enmeshed_events_processing_duration_seconds", unit: "s", advice: new InstrumentAdvice<double>
        {
            HistogramBucketBoundaries = [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10, 30, 60, 180, 600]
        });
        _numberOfProcessingErrors = meter.CreateCounter<long>(name: "enmeshed_events_processing_errors_total");

        _numberOfPublishedEvents = meter.CreateCounter<long>(name: "enmeshed_events_published_total");
        _eventPublishingDuration = meter.CreateHistogram(name: "enmeshed_events_publishing_duration_seconds", unit: "s", advice: new InstrumentAdvice<double>
        {
            HistogramBucketBoundaries = [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5]
        });
        _messageSizeBytes = meter.CreateHistogram(name: "enmeshed_events_message_size_bytes", unit: "By", advice: new InstrumentAdvice<int>
        {
            HistogramBucketBoundaries = [100, 250, 500, 750, 1000, 2500]
        });
        _numberOfPublishingErrors = meter.CreateCounter<long>(name: "enmeshed_events_publishing_errors_total");
    }

    #region processing

    public void TrackEventProcessingDuration(long startedAt, string queueName)
    {
        _eventProcessingDuration.Record(
            Stopwatch.GetElapsedTime(startedAt).TotalSeconds,
            new KeyValuePair<string, object?>("queue_name", queueName));
    }

    public void IncrementNumberOfHandledEvents(string queueName)
    {
        _numberOfProcessedEvents.Add(
            1,
            new KeyValuePair<string, object?>("queue_name", queueName)
        );
    }

    public void IncrementNumberOfProcessingErrors(string queueName)
    {
        _numberOfProcessingErrors.Add(
            1,
            new KeyValuePair<string, object?>("queue_name", queueName)
        );
    }

    #endregion

    #region publishing

    public void TrackEventPublishingDuration(long startedAt)
    {
        _eventPublishingDuration.Record(Stopwatch.GetElapsedTime(startedAt).TotalSeconds);
    }

    public void IncrementNumberOfPublishedEvents(string eventName)
    {
        _numberOfPublishedEvents.Add(
            1,
            new KeyValuePair<string, object?>("event_name", eventName)
        );
    }

    public void TrackHandledMessageSize(int messageSize)
    {
        _messageSizeBytes.Record(messageSize);
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

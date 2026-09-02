using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Backbone.BuildingBlocks.Application.Housekeeping;

public class HousekeepingTelemetry
{
    private readonly Histogram<double> _deletionDuration;
    private readonly Counter<long> _numberOfDeletedItems;

    public HousekeepingTelemetry(Meter meter)
    {
        _numberOfDeletedItems = meter.CreateCounter<long>(name: "enmeshed_housekeeping_deleted_items_total");
        _deletionDuration = meter.CreateHistogram(name: "enmeshed_housekeeping_deletion_duration_seconds", unit: "s", advice: new InstrumentAdvice<double>
        {
            HistogramBucketBoundaries = [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10, 30, 60, 180, 600]
        });
    }

    public async Task TrackDeletion(string itemType, Func<CancellationToken, Task<int>> delete, CancellationToken cancellationToken)
    {
        using var activity = StartDeletionActivity(itemType);
        var startedAt = Stopwatch.GetTimestamp();

        try
        {
            var numberOfDeletedItems = await delete(cancellationToken);
            activity?.SetTag("housekeeping.deleted_items", numberOfDeletedItems);

            TrackDeletion(numberOfDeletedItems, startedAt, itemType);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            activity?.SetTag("error.type", ex.GetType().Name);
            throw;
        }
    }

    private void TrackDeletion(int numberOfDeletedItems, long startedAt, string itemType)
    {
        var itemTypeTag = new KeyValuePair<string, object?>("item_type", itemType);

        _numberOfDeletedItems.Add(numberOfDeletedItems, itemTypeTag);
        _deletionDuration.Record(Stopwatch.GetElapsedTime(startedAt).TotalSeconds, itemTypeTag);
    }

    private static Activity? StartDeletionActivity(string itemType)
    {
        var activity = HousekeepingDiagnostics.ACTIVITY_SOURCE.StartActivity($"housekeeping delete {itemType}", ActivityKind.Internal);

        if (activity == null)
            return null;

        activity.SetTag("housekeeping.operation", "delete");
        activity.SetTag("housekeeping.item_type", itemType);

        return activity;
    }
}

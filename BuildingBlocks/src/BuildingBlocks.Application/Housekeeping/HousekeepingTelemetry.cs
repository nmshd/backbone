using System.Diagnostics;

namespace Backbone.BuildingBlocks.Application.Housekeeping;

public class HousekeepingTelemetry
{
    public async Task TrackDeletion(string itemType, Func<CancellationToken, Task<int>> delete, CancellationToken cancellationToken)
    {
        using var activity = StartDeletionActivity(itemType);

        try
        {
            var numberOfDeletedItems = await delete(cancellationToken);
            activity?.SetTag("housekeeping.deleted_items", numberOfDeletedItems);
        }
        catch (Exception ex)
        {
            TrackException(activity, ex);
            throw;
        }
    }

    public async Task TrackCommand(string moduleName, Func<CancellationToken, Task> execute, CancellationToken cancellationToken)
    {
        using var activity = StartModuleDeletionRunActivity(moduleName);

        try
        {
            await execute(cancellationToken);
        }
        catch (Exception ex)
        {
            TrackException(activity, ex);
            throw;
        }
    }

    private static Activity? StartModuleDeletionRunActivity(string moduleName)
    {
        var activity = HousekeepingDiagnostics.ACTIVITY_SOURCE.StartActivity(moduleName);

        if (activity == null)
            return null;

        activity.SetTag("housekeeping.operation", "module_deletion_run");
        activity.SetTag("housekeeping.module", moduleName);

        return activity;
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

    private static void TrackException(Activity? activity, Exception exception)
    {
        activity?.AddException(exception);
        activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
    }
}

using System.Diagnostics;

namespace Backbone.BuildingBlocks.Application.Housekeeping;

public static class HousekeepingTelemetry
{
    public const string ACTIVITY_SOURCE_NAME = "Backbone.Housekeeping";

    public static readonly ActivitySource ACTIVITY_SOURCE = new(ACTIVITY_SOURCE_NAME);

    public static async Task TrackModuleDeletion(string moduleName, Func<CancellationToken, Task> execute, CancellationToken cancellationToken)
    {
        using var activity = StartModuleDeletionActivity(moduleName);

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

    private static Activity? StartModuleDeletionActivity(string moduleName)
    {
        var activity = ACTIVITY_SOURCE.StartActivity(moduleName);

        if (activity == null)
            return null;

        activity.SetTag("housekeeping.operation", "module_deletion");
        activity.SetTag("housekeeping.module", moduleName);

        return activity;
    }

    public static async Task TrackItemDeletion(string itemType, Func<CancellationToken, Task<int>> delete, CancellationToken cancellationToken)
    {
        using var activity = StartItemDeletionActivity(itemType);

        try
        {
            var numberOfDeletedItems = await delete(cancellationToken);
            activity?.SetTag("housekeeping.deleted_item_count", numberOfDeletedItems);
        }
        catch (Exception ex)
        {
            TrackException(activity, ex);
            throw;
        }
    }

    private static Activity? StartItemDeletionActivity(string itemType)
    {
        var activity = ACTIVITY_SOURCE.StartActivity($"delete {itemType}");

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

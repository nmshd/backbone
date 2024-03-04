using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application;

public static class ApplicationErrors
{
    public static class Datawallet
    {
        public static ApplicationError DatawalletNotUpToDate(long? localIndex = -100, long latestIndex = -100)
        {
            var localIndexString = localIndex == -100 ? " " : $" '{localIndex?.ToString() ?? "null"}' ";
            var latestIndexString = localIndex == -100 ? "" : $" ('{latestIndex}') ";

            return new ApplicationError("error.platform.validation.datawallet.datawalletNotUpToDate", $"The sent localIndex{localIndexString}does not match the index of the latest modification{latestIndexString}. This probably means that your local datawallet is not up to date. Make sure you applied all modifications from the backbone before pushing a new modification.");
        }

        public static ApplicationError CannotPushModificationsDuringActiveSyncRun()
        {
            return new ApplicationError("error.platform.validation.datawallet.cannotPushDatawalletModificationsDuringActiveSyncRun", "There is an active sync run. In order to avoid conflicts it is not allowed to push new modifications while a sync run is active. Try again in a few seconds.");
        }

        public static ApplicationError InsufficientSupportedDatawalletVersion()
        {
            return new ApplicationError("error.platform.validation.datawallet.insufficientSupportedDatawalletVersion", "The supported datawallet version sent with the request is lower than the actual version of the datawallet.");
        }
    }

    public static class SyncRuns
    {
        public static ApplicationError SyncRunAlreadyFinalized()
        {
            return new ApplicationError("error.platform.validation.syncRun.syncRunAlreadyFinalized", "This sync run is already finalized.");
        }

        public static ApplicationError CannotFinalizeSyncRunStartedByAnotherDevice()
        {
            return new ApplicationError("error.platform.validation.syncRun.cannotFinalizeSyncRunStartedByAnotherDevice", "This sync was started by another device. You can only finalize a sync run with the device it was started from.");
        }

        public static ApplicationError CannotRefreshExpirationTimeOfSyncRunStartedByAnotherDevice()
        {
            return new ApplicationError("error.platform.validation.syncRun.cannotRefreshExpirationTimeOfSyncRunStartedByAnotherDevice", "This sync was started by another device. You can only extend the expiration time of a sync run with the device it was started from.");
        }

        public static ApplicationError CannotReadExternalEventsOfSyncRunStartedByAnotherDevice()
        {
            return new ApplicationError("error.platform.validation.syncRun.cannotReadExternalEventsOfASyncRunStartedByAnotherDevice", "This sync was started by another device. Only that device can read the corresponding external events.");
        }

        public static ApplicationError CannotStartSyncRunWhenAnotherSyncRunIsRunning(SyncRunId? idOfRunningSyncRun = null)
        {
            var idString = idOfRunningSyncRun == null ? "" : $" (ID: {idOfRunningSyncRun})";
            return new ApplicationError("error.platform.validation.syncRun.cannotStartSyncRunWhenAnotherSyncRunIsRunning", $"Another sync run is currently active{idString}. There can only be one active sync run at a time. Try again in a few seconds.");
        }

        public static ApplicationError UnexpectedSyncRunType(SyncRun.SyncRunType expectedType)
        {
            return new ApplicationError("error.platform.validation.syncRun.unexpectedSyncRunType", $"The current operation only supports sync runs of type '{expectedType}'.");
        }
    }

    public class Generic
    {
        public static ApplicationError OperationFailed(string message = "Operation failed.")
        {
            return new ApplicationError("error.platform.operationFailed", message);
        }
    }
}

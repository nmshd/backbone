namespace Synchronization.Domain.Entities.Sync;

public class SyncError
{
#pragma warning disable CS8618
    private SyncError() { }
#pragma warning restore CS8618

    public SyncError(SyncRun syncRun, ExternalEvent externalEvent, string errorCode)
    {
        Id = SyncErrorId.New();
        SyncRunId = syncRun.Id;
        ExternalEventId = externalEvent.Id;

        SyncRun = syncRun;
        ExternalEvent = externalEvent;
        ErrorCode = errorCode;
    }

    public SyncErrorId Id { get; }
    public SyncRunId SyncRunId { get; }
    public ExternalEventId ExternalEventId { get; }

    public SyncRun SyncRun { get; }
    public ExternalEvent ExternalEvent { get; }

    public string ErrorCode { get; }
}

using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class SyncError : Entity<SyncErrorId>
{
    // ReSharper disable once UnusedMember.Local
    private SyncError()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        SyncRunId = null!;
        ExternalEventId = null!;
        SyncRun = null!;
        ExternalEvent = null!;
        ErrorCode = null!;
    }

    public SyncError(SyncRun syncRun, ExternalEvent externalEvent, string errorCode) : base(SyncErrorId.New())
    {
        SyncRunId = syncRun.Id;
        ExternalEventId = externalEvent.Id;

        SyncRun = syncRun;
        ExternalEvent = externalEvent;
        ErrorCode = errorCode;
    }

    public SyncRunId SyncRunId { get; }
    public ExternalEventId ExternalEventId { get; }

    public SyncRun SyncRun { get; }
    public ExternalEvent ExternalEvent { get; }

    public string ErrorCode { get; }
}

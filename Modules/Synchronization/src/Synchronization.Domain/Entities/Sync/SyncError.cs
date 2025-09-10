using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class SyncError : Entity
{
    // ReSharper disable once UnusedMember.Local
    protected SyncError()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        SyncRunId = null!;
        ExternalEventId = null!;
        SyncRun = null!;
        ExternalEvent = null!;
        ErrorCode = null!;
    }

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
    public SyncRunId? SyncRunId { get; }
    public ExternalEventId ExternalEventId { get; }

    public virtual SyncRun SyncRun { get; }
    public virtual ExternalEvent ExternalEvent { get; }

    public string ErrorCode { get; }
}

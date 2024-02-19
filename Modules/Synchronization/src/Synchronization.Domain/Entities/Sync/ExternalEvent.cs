using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class ExternalEvent
{
    private readonly List<SyncError> _errors = new();

#pragma warning disable CS8618
    private ExternalEvent() { }
#pragma warning restore CS8618

    public ExternalEvent(ExternalEventType type, IdentityAddress owner, long index, object payload)
    {
        Id = ExternalEventId.New();
        Type = type;
        Index = index;
        Owner = owner;
        CreatedAt = SystemTime.UtcNow;
        Payload = payload;
    }

    public ExternalEventId Id { get; }
    public ExternalEventType Type { get; }
    public long Index { get; }

    public IdentityAddress Owner { get; }
    public DateTime CreatedAt { get; }

    public object Payload { get; }

    public byte SyncErrorCount { get; internal set; }
    public SyncRun? SyncRun { get; private set; }
    public SyncRunId? SyncRunId { get; private set; }
    public IReadOnlyCollection<SyncError> Errors => _errors;

    public void AssignToSyncRun(SyncRun syncRun)
    {
        SyncRun = syncRun;
        SyncRunId = syncRun.Id;
    }

    public void SyncFailed(SyncError error)
    {
        SyncRunId = null;
        SyncErrorCount++;
        _errors.Add(error);
    }
}

public enum ExternalEventType
{
    MessageReceived = 0,
    MessageDelivered = 1,
    RelationshipChangeCreated = 2,
    RelationshipChangeCompleted = 3,
    IdentityDeletionProcessStarted = 4
}

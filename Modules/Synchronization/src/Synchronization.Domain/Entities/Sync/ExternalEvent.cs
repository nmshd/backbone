using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class ExternalEvent : Entity
{
    private readonly List<SyncError> _errors = [];

    // ReSharper disable once UnusedMember.Local
    protected ExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        Owner = null!;
        Payload = null!;
    }

    protected ExternalEvent(ExternalEventType type, IdentityAddress owner, object payload)
    {
        Id = ExternalEventId.New();
        Type = type;
        Owner = owner;
        CreatedAt = SystemTime.UtcNow;
        Payload = payload;

        RaiseDomainEvent(new ExternalEventCreatedDomainEvent(this));
    }

    public ExternalEventId Id { get; }
    public ExternalEventType Type { get; }
    public long Index { get; private set; }

    public IdentityAddress Owner { get; }
    public DateTime CreatedAt { get; }

    public object Payload { get; }

    public byte SyncErrorCount { get; internal set; }
    public SyncRun? SyncRun { get; private set; }
    public SyncRunId? SyncRunId { get; private set; }
    public IReadOnlyCollection<SyncError> Errors => _errors;

    public void UpdateIndex(long newIndex)
    {
        Index = newIndex;
    }

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

    RelationshipStatusChanged = 10,
    RelationshipReactivationRequested = 12,
    RelationshipReactivationCompleted = 13,

    IdentityDeletionProcessStarted = 20,
    IdentityDeletionProcessStatusChanged = 21,
    PeerToBeDeleted = 22,
    PeerDeletionCancelled = 23,
    PeerDeleted = 24
}

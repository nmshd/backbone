using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class ExternalEvent : Entity
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected ExternalEvent()
    {
        Context = null!;
        Id = null!;
        Owner = null!;
        Payload = null!;
        Errors = null!;
    }

    protected ExternalEvent(ExternalEventType type, IdentityAddress owner, object payload, string? context = null)
    {
        Id = ExternalEventId.New();
        Type = type;
        Owner = owner;
        CreatedAt = SystemTime.UtcNow;
        Payload = payload;
        Context = context;
        Errors = [];

        RaiseDomainEvent(new ExternalEventCreatedDomainEvent(this));
    }

    public ExternalEventId Id { get; }
    public ExternalEventType Type { get; }
    public long Index { get; private set; }

    public IdentityAddress Owner { get; }
    public DateTime CreatedAt { get; }

    public object Payload { get; }

    public byte SyncErrorCount { get; internal set; }
    public virtual SyncRun? SyncRun { get; private set; }
    public SyncRunId? SyncRunId { get; private set; }
    public virtual List<SyncError> Errors { get; }

    public string? Context { get; }
    public bool IsDeliveryBlocked { get; private set; }

    public void UpdateIndex(long newIndex)
    {
        Index = newIndex;
    }

    public void BlockDelivery()
    {
        IsDeliveryBlocked = true;
    }

    public void UnblockDelivery()
    {
        IsDeliveryBlocked = false;
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
        Errors.Add(error);
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
    PeerDeleted = 24,

    TokenLocked = 30,

    PeerFeatureFlagsChanged = 40,

    RelationshipTemplateAllocationsExhausted = 50,

    FileOwnershipLocked = 60,
    FileOwnershipClaimed = 61
}

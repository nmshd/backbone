using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class ExternalEvent : Entity<ExternalEventId>
{
    private readonly List<SyncError> _errors = [];

    // ReSharper disable once UnusedMember.Local
    private ExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Owner = null!;
        Payload = null!;
    }

    public ExternalEvent(ExternalEventType type, IdentityAddress owner, long index, object payload) : base(ExternalEventId.New())
    {
        Type = type;
        Index = index;
        Owner = owner;
        CreatedAt = SystemTime.UtcNow;
        Payload = payload;

        RaiseDomainEvent(new ExternalEventCreatedDomainEvent(this));
    }

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
    IdentityDeletionProcessStarted = 4,
    IdentityDeletionProcessStatusChanged = 5
}

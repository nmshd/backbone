using Backbone.BuildingBlocks.Domain;
using Backbone.Tooling;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class SyncError : Entity
{
    // ReSharper disable once UnusedMember.Local
    protected SyncError()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        ExternalEventId = null!;
        ErrorCode = null!;
    }

    public SyncError(ExternalEventId externalEventId, string errorCode)
    {
        Id = SyncErrorId.New();
        ExternalEventId = externalEventId;

        ErrorCode = errorCode;
        CreatedAt = SystemTime.UtcNow;
    }

    public SyncErrorId Id { get; }
    public ExternalEventId ExternalEventId { get; }

    public string ErrorCode { get; }
    public DateTime CreatedAt { get; }
}

using Backbone.BuildingBlocks.Domain;
using Backbone.Tooling;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class SyncError : Entity
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected SyncError()
    {
        Id = null!;
        ExternalEventId = null!;
        ErrorCode = null!;
    }

    public SyncError(ExternalEvent externalEvent, string errorCode)
    {
        Id = SyncErrorId.New();
        ExternalEventId = externalEvent.Id;

        ErrorCode = errorCode;
        CreatedAt = SystemTime.UtcNow;
    }

    public SyncErrorId Id { get; }
    public ExternalEventId ExternalEventId { get; }

    public string ErrorCode { get; }
    public DateTime CreatedAt { get; }
}

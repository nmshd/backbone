using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class FileOwnershipLockedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected FileOwnershipLockedExternalEvent()
    {
    }

    public FileOwnershipLockedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.FileOwnershipLocked, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string FileId { get; init; }
    }
}

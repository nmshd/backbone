using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class FileOwnershipClaimedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected FileOwnershipClaimedExternalEvent()
    {
    }

    public FileOwnershipClaimedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.FileOwnershipClaimed, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string FileId { get; init; }
        public required string NewOwnerAddress { get; init; }
    }
}

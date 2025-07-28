using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class FileOwnershipClaimedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    protected FileOwnershipClaimedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
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

using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class FileOwnershipLockedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    private FileOwnershipLockedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
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

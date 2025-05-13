using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class FileOwnershipIsLockedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    private FileOwnershipIsLockedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
    }

    public FileOwnershipIsLockedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.FileOwnershipIsLockedEvent, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string FileAddress { get; init; }
    }
}

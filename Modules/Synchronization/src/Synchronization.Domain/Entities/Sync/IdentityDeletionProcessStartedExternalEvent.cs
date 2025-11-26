using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class IdentityDeletionProcessStartedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected IdentityDeletionProcessStartedExternalEvent()
    {
    }

    public IdentityDeletionProcessStartedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.IdentityDeletionProcessStarted, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string DeletionProcessId { get; init; }
    }
}

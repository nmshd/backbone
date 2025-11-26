using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class IdentityDeletionProcessStatusChangedExternalEvent : ExternalEvent
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected IdentityDeletionProcessStatusChangedExternalEvent()
    {
    }

    public IdentityDeletionProcessStatusChangedExternalEvent(IdentityAddress owner, EventPayload payload)
        : base(ExternalEventType.IdentityDeletionProcessStatusChanged, owner, payload)
    {
    }

    public record EventPayload
    {
        public required string DeletionProcessId { get; init; }
    }
}

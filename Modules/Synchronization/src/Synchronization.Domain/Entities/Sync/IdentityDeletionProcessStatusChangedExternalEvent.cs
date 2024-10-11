using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class IdentityDeletionProcessStatusChangedExternalEvent : ExternalEvent
{
    public IdentityDeletionProcessStatusChangedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.IdentityDeletionProcessStatusChanged, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string DeletionProcessId { get; init; }
    }
}

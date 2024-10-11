using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class IdentityDeletionProcessStartedExternalEvent : ExternalEvent
{
    public IdentityDeletionProcessStartedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.IdentityDeletionProcessStarted, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string DeletionProcessId { get; init; }
    }
}

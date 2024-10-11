using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class IdentityDeletionProcessStartedExternalEvent : ExternalEvent
{
    // ReSharper disable once UnusedMember.Local
    private IdentityDeletionProcessStartedExternalEvent()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
    }
    
    public IdentityDeletionProcessStartedExternalEvent(IdentityAddress owner, PayloadT payload)
        : base(ExternalEventType.IdentityDeletionProcessStarted, owner, payload)
    {
    }

    public record PayloadT
    {
        public required string DeletionProcessId { get; init; }
    }
}

using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class IdentityDeletionProcessStartedDomainEvent : DomainEvent
{
    public IdentityDeletionProcessStartedDomainEvent(string identityAddress, string deletionProcessId, string? initiator) : base($"{identityAddress}/DeletionProcessStarted/{deletionProcessId}")
    {
        DeletionProcessId = deletionProcessId;
        Address = identityAddress;
        Initiator = initiator;
    }

    public string Address { get; init; }
    public string DeletionProcessId { get; init; }
    public string? Initiator { get; set; }
}

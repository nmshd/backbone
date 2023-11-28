using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;

public class IdentityDeletionProcessStartedIntegrationEvent : IntegrationEvent
{
    public IdentityDeletionProcessStartedIntegrationEvent(Identity identity, IdentityDeletionProcess deletionProcess) : base($"{identity.Address}/DeletionProcessStarted")
    {
        DeletionProcessId = deletionProcess.Id;
        Address = identity.Address;
    }

    public string Address { get; }
    public string DeletionProcessId { get; }
}

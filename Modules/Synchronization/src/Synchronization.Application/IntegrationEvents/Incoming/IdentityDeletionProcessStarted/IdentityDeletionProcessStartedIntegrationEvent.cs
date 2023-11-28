using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStarted;

public class IdentityDeletionProcessStartedIntegrationEvent : IntegrationEvent
{
    public string Address { get; }
    public string DeletionProcessId { get; }
}

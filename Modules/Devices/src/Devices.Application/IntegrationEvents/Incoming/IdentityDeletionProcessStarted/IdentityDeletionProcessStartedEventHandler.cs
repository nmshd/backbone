using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStarted;

public class IdentityDeletionProcessStartedEventHandler : IIntegrationEventHandler<IdentityDeletionProcessStartedEvent>
{
    public Task Handle(IdentityDeletionProcessStartedEvent @event)
    {
        throw new NotImplementedException();
    }
}

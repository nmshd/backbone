using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Incoming;

public class PeerIdentityToBeDeletedIntegrationEventHandler : IIntegrationEventHandler<PeerIdentityToBeDeletedIntegrationEvent>
{
    public Task Handle(PeerIdentityToBeDeletedIntegrationEvent @event)
    {
        throw new NotImplementedException();
    }
}

using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Incoming;

public class IdentityToBeDeletedIntegrationEventHandler : IIntegrationEventHandler<IdentityToBeDeletedIntegrationEvent>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventBus _eventBus;

    public IdentityToBeDeletedIntegrationEventHandler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        _relationshipsRepository = relationshipsRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(IdentityToBeDeletedIntegrationEvent @event)
    {
        var relationships = await _relationshipsRepository.FindRelationships(Relationship.HasParticipant(@event.Address), CancellationToken.None);
        foreach (var relationship in relationships)
        {
            _eventBus.Publish(new PeerIdentityToBeDeletedIntegrationEvent(@event.Address, @event.DeletionProcessId, relationship.Id));
        }
    }
}

using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming.PeerIdentityDeleted;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.PeerIdentityDeleted;
public class PeerIdentityDeletedDomainEventHandler : IDomainEventHandler<PeerIdentityDeletedDomainEvent>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventBus _eventBus;

    public PeerIdentityDeletedDomainEventHandler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        _relationshipsRepository = relationshipsRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(PeerIdentityDeletedDomainEvent request)
    {
        var relationshipId = RelationshipId.Parse(request.RelationshipId);
        var relationship = await _relationshipsRepository.FindRelationship(relationshipId, request.IdentityAddress, CancellationToken.None, track: true);

        var identity = relationship.To == request.IdentityAddress ? relationship.From : relationship.To;

        _eventBus.Publish(new PeerFromRelationshipDeletedDomainEvent(identity, relationship.Id, request.IdentityAddress));
    }
}

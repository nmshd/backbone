using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming.IdentityToBeDeleted;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityToBeDeleted;
public class IdentityToBeDeletedDomainEventHandler : IDomainEventHandler<IdentityToBeDeletedDomainEvent>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventBus _eventBus;

    public IdentityToBeDeletedDomainEventHandler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        _relationshipsRepository = relationshipsRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(IdentityToBeDeletedDomainEvent @event)
    {
        var relationships = await FindRelationshipsOfPeer(@event.IdentityAddress);

        NotifyRelationshipsOfPeerToBeDeleted(@event, relationships);
    }

    private void NotifyRelationshipsOfPeerToBeDeleted(IdentityToBeDeletedDomainEvent @event, IEnumerable<Relationship> relationships)
    {
        foreach (var relationship in relationships)
        {
            var identity = relationship.To == @event.IdentityAddress ? relationship.From : relationship.To;

            _eventBus.Publish(new PeerToBeDeletedDomainEvent(identity, relationship.Id, @event.IdentityAddress));
        }
    }

    private async Task<IEnumerable<Relationship>> FindRelationshipsOfPeer(string identityAddress)
    {
        var relationships = await _relationshipsRepository
            .FindRelationships(r => r.From == identityAddress || r.To == identityAddress, CancellationToken.None);
        return relationships;
    }
}

using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming.PeerIdentityToBeDeleted;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.PeerIdentityToBeDeleted;
internal class PeerIdentityToBeDeletedDomainEventHandler : IDomainEventHandler<PeerIdentityToBeDeletedDomainEvent>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventBus _eventBus;

    public PeerIdentityToBeDeletedDomainEventHandler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        _relationshipsRepository = relationshipsRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(PeerIdentityToBeDeletedDomainEvent @event)
    {
        var relationships = await _relationshipsRepository
            .FindRelationships(r => r.From == @event.IdentityAddress || r.To == @event.IdentityAddress, CancellationToken.None);

        foreach (var relationship in relationships)
        {
            var identity = relationship.To == @event.IdentityAddress ? relationship.From : relationship.To;

            _eventBus.Publish(new PeerIsToBeDeletedDomainEvent(identity, relationship.Id, @event.IdentityAddress));
        }
    }
}

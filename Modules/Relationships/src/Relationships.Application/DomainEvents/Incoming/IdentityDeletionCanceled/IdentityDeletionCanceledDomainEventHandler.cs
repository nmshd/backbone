using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming.IdentityDeletionCanceled;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityDeletionCanceled;
public class IdentityDeletionCanceledDomainEventHandler : IDomainEventHandler<IdentityDeletionCanceledDomainEvent>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventBus _eventBus;

    public IdentityDeletionCanceledDomainEventHandler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        _relationshipsRepository = relationshipsRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(IdentityDeletionCanceledDomainEvent @event)
    {
        var relationships = await FindRelationshipsOfPeer(@event.IdentityAddress);

        NotifyRelationshipsOfPeerDeletionCanceled(@event, relationships);
    }

    private void NotifyRelationshipsOfPeerDeletionCanceled(IdentityDeletionCanceledDomainEvent @event, IEnumerable<Relationship> relationships)
    {
        foreach (var relationship in relationships)
        {
            var identity = relationship.To == @event.IdentityAddress ? relationship.From : relationship.To;

            _eventBus.Publish(new PeerDeletionCanceledDomainEvent(identity, relationship.Id, @event.IdentityAddress));
        }
    }

    private async Task<IEnumerable<Relationship>> FindRelationshipsOfPeer(string identityAddress)
    {
        return await _relationshipsRepository
            .FindRelationships(r => r.From == identityAddress || r.To == identityAddress, CancellationToken.None);
    }
}

using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming.IdentityDeleted;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityDeleted;
public class IdentityDeletedDomainEventHandler : IDomainEventHandler<IdentityDeletedDomainEvent>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventBus _eventBus;

    public IdentityDeletedDomainEventHandler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        _relationshipsRepository = relationshipsRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(IdentityDeletedDomainEvent @event)
    {
        var relationships = await GetRelationshipsOfDeletedPeer(@event.IdentityAddress);

        NotifyRelationshipsOfPeerDeleted(@event, relationships);
    }

    private void NotifyRelationshipsOfPeerDeleted(IdentityDeletedDomainEvent @event, IEnumerable<Relationship> relationships)
    {
        foreach (var relationship in relationships)
        {
            var identity = relationship.To == @event.IdentityAddress ? relationship.From : relationship.To;

            _eventBus.Publish(new PeerDeletedDomainEvent(identity, relationship.Id, @event.IdentityAddress));
        }
    }

    private async Task<IEnumerable<Relationship>> GetRelationshipsOfDeletedPeer(string identityAddress)
    {
        return await _relationshipsRepository
            .FindRelationships(r => r.From == identityAddress || r.To == identityAddress, CancellationToken.None);
    }
}

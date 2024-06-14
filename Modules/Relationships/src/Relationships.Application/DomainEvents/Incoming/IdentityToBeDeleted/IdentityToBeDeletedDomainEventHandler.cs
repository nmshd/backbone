using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityToBeDeleted;
public class IdentityToBeDeletedDomainEventHandler : IDomainEventHandler<IdentityToBeDeletedDomainEvent>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public IdentityToBeDeletedDomainEventHandler(IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task Handle(IdentityToBeDeletedDomainEvent @event)
    {
        var relationships = await GetRelationshipsOfPeer(@event.IdentityAddress);

        NotifyRelationshipsOfPeerToBeDeleted(@event.IdentityAddress, relationships);
    }

    private static void NotifyRelationshipsOfPeerToBeDeleted(string identityToBeDeleted, IEnumerable<Relationship> relationships)
    {
        foreach (var relationship in relationships)
        {
            relationship.RaisePeerToBeDeletedDomainEvent(identityToBeDeleted);
        }
    }

    private async Task<IEnumerable<Relationship>> GetRelationshipsOfPeer(string identityAddress)
    {
        var relationships = await _relationshipsRepository
            .FindRelationships(r => (r.From == identityAddress || r.To == identityAddress) && r.Status == RelationshipStatus.Active,
                CancellationToken.None);
        return relationships;
    }
}

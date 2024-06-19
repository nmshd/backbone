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
        var relationships = await GetRelationshipsOf(@event.IdentityAddress);

        NotifyRelationshipsOfPeerToBeDeleted(@event.IdentityAddress, relationships);

        await _relationshipsRepository.Update(relationships);
    }

    private async Task<List<Relationship>> GetRelationshipsOf(string identityAddress)
    {
        var relationships = (await _relationshipsRepository
            .FindRelationships(r => (r.From == identityAddress || r.To == identityAddress) && r.Status == RelationshipStatus.Active,
                CancellationToken.None)).ToList();

        return relationships;
    }

    private static void NotifyRelationshipsOfPeerToBeDeleted(string identityToBeDeleted, IEnumerable<Relationship> relationships)
    {
        foreach (var relationship in relationships)
        {
            relationship.ParticipantIsToBeDeleted(identityToBeDeleted);
        }
    }
}

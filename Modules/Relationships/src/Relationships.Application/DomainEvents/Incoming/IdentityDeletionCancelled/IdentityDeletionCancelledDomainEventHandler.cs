using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityDeletionCancelled;

public class IdentityDeletionCancelledDomainEventHandler : IDomainEventHandler<IdentityDeletionCancelledDomainEvent>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public IdentityDeletionCancelledDomainEventHandler(IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task Handle(IdentityDeletionCancelledDomainEvent @event)
    {
        var relationships = await GetRelationshipsOf(@event.IdentityAddress);

        NotifyRelationshipsAboutCancelledDeletion(@event.IdentityAddress, relationships);
    }

    private async Task<IEnumerable<Relationship>> GetRelationshipsOf(string identityAddress)
    {
        var relationships = await _relationshipsRepository
            .FindRelationships(r => (r.From == identityAddress || r.To == identityAddress) && r.Status == RelationshipStatus.Active,
                CancellationToken.None);
        return relationships;
    }

    private static void NotifyRelationshipsAboutCancelledDeletion(string identityToBeDeleted, IEnumerable<Relationship> relationships)
    {
        foreach (var relationship in relationships)
        {
            relationship.DeletionOfParticipantCancelled(identityToBeDeleted);
        }
    }
}

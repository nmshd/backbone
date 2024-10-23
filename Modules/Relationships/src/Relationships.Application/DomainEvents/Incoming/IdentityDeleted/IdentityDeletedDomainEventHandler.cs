using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityDeleted;

public class IdentityDeletedDomainEventHandler : IDomainEventHandler<IdentityDeletedDomainEvent>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public IdentityDeletedDomainEventHandler(IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task Handle(IdentityDeletedDomainEvent @event)
    {
        var relationships = await GetRelationshipsOf(@event.IdentityAddress);

        NotifyRelationshipsAboutDeletedPeer(@event.IdentityAddress, relationships);

        await _relationshipsRepository.Update(relationships);
    }

    private async Task<List<Relationship>> GetRelationshipsOf(string identityAddress)
    {
        var relationships = (await _relationshipsRepository
            .FindRelationships(
                Relationship.HasParticipant(identityAddress).And(Relationship.HasStatusInWhichPeerShouldBeNotifiedAboutDeletion()),
                CancellationToken.None)).ToList();
        return relationships;
    }

    private static void NotifyRelationshipsAboutDeletedPeer(string identityToBeDeleted, IEnumerable<Relationship> relationships)
    {
        foreach (var relationship in relationships)
        {
            relationship.DeletionOfParticipantStarted(identityToBeDeleted);
        }
    }
}

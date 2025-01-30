﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
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
        var relationships = await GetRelationshipsOf(@event.IdentityAddress);

        NotifyRelationshipsOfPeerToBeDeleted(@event.IdentityAddress, relationships, @event.GracePeriodEndsAt);

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

    private void NotifyRelationshipsOfPeerToBeDeleted(string identityToBeDeleted, IEnumerable<Relationship> relationships, DateTime gracePeriodEndsAt)
    {
        foreach (var relationship in relationships)
        {
            _eventBus.Publish(new PeerToBeDeletedDomainEvent(relationship.GetPeerOf(identityToBeDeleted), relationship.Id, identityToBeDeleted, gracePeriodEndsAt));
        }
    }
}

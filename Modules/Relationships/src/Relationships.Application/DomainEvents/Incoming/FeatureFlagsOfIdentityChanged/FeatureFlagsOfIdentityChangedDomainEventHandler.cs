﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.FeatureFlagsOfIdentityChanged;

public class FeatureFlagsOfIdentityChangedDomainEventHandler : IDomainEventHandler<FeatureFlagsOfIdentityChangedDomainEvent>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventBus _eventBus;

    public FeatureFlagsOfIdentityChangedDomainEventHandler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IEventBus eventBus, IRelationshipsRepository relationshipsRepository)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _eventBus = eventBus;
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task Handle(FeatureFlagsOfIdentityChangedDomainEvent @event)
    {
        var identitiesToBeNotified = await FindAllIdentitiesToBeNotified(@event);

        var publishEventTasks = identitiesToBeNotified.Select(i => _eventBus.Publish(new PeerFeatureFlagsChangedDomainEvent
        {
            PeerAddress = @event.IdentityAddress,
            NotifiedIdentityAddress = i
        }));

        await Task.WhenAll(publishEventTasks);
    }

    private async Task<HashSet<IdentityAddress>> FindAllIdentitiesToBeNotified(FeatureFlagsOfIdentityChangedDomainEvent @event)
    {
        var identitiesToBeNotified = new HashSet<IdentityAddress>();

        var activeAndPendingRelationshipAddressPairs = await
            _relationshipsRepository.FindRelationships(
                Relationship.HasParticipant(@event.IdentityAddress).And(Relationship.HasStatusInWhichPeerShouldBeNotifiedAboutDeletion()),
                r => new { r.From, r.To },
                CancellationToken.None);

        foreach (var addressPair in activeAndPendingRelationshipAddressPairs)
            identitiesToBeNotified.Add(addressPair.From == @event.IdentityAddress ? addressPair.To : addressPair.From);

        var allocatorAddresses = await _relationshipTemplatesRepository.FindRelationshipTemplateAllocations(
            RelationshipTemplateAllocation.BelongsToTemplateCreatedBy(@event.IdentityAddress),
            a => a.AllocatedBy, CancellationToken.None);

        identitiesToBeNotified.UnionWith(allocatorAddresses);

        return identitiesToBeNotified;
    }
}



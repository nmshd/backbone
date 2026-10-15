using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Domain.DomainEvents;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Relationships.Module.Features.DomainEvents.FeatureFlagsOfIdentityChanged;

public class Handler : IDomainEventHandler<FeatureFlagsOfIdentityChangedDomainEvent>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventOutbox<RelationshipsModule> _outbox;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IEventOutbox<RelationshipsModule> outbox, IRelationshipsRepository relationshipsRepository)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _outbox = outbox;
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task Handle(FeatureFlagsOfIdentityChangedDomainEvent @event)
    {
        var identitiesToBeNotified = await ListIdentitiesToBeNotified(@event);

        await _outbox.Enqueue(identitiesToBeNotified.Select(identity =>
            ((DomainEvent)PeerFeatureFlagsChangedDomainEvent.Create(@event.IdentityAddress, identity), $"peer-notification:{identity}")));
    }

    private async Task<HashSet<IdentityAddress>> ListIdentitiesToBeNotified(FeatureFlagsOfIdentityChangedDomainEvent @event)
    {
        var identitiesToBeNotified = new HashSet<IdentityAddress>();

        var activeAndPendingRelationshipAddressPairs = await
            _relationshipsRepository.ListWithoutContent(
                Relationship.HasParticipant(@event.IdentityAddress).And(Relationship.HasStatusInWhichPeerShouldBeNotifiedAboutFeatureFlagsChange()),
                r => new { r.From, r.To },
                CancellationToken.None);

        foreach (var relationshipParticipant in activeAndPendingRelationshipAddressPairs)
            identitiesToBeNotified.Add(@event.IdentityAddress == relationshipParticipant.From ? relationshipParticipant.To : relationshipParticipant.From);

        var allocatorAddresses = await _relationshipTemplatesRepository.ListRelationshipTemplateAllocations(
            RelationshipTemplateAllocation.BelongsToTemplateCreatedBy(@event.IdentityAddress),
            a => a.AllocatedBy, CancellationToken.None);

        identitiesToBeNotified.UnionWith(allocatorAddresses);

        return identitiesToBeNotified;
    }
}

using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityDeletionCancelled;

public class Handler : IDomainEventHandler<IdentityDeletionCancelledDomainEvent>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventBus _eventBus;

    public Handler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        _relationshipsRepository = relationshipsRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(IdentityDeletionCancelledDomainEvent @event)
    {
        var relationships = await GetRelationshipsOf(@event.IdentityAddress);

        await NotifyRelationshipsAboutCancelledDeletion(@event.IdentityAddress, relationships);
    }

    private async Task<List<Relationship>> GetRelationshipsOf(string identityAddress)
    {
        var relationships = (await _relationshipsRepository
            .ListWithoutContent(
                Relationship.HasParticipant(identityAddress).And(Relationship.HasStatusInWhichPeerShouldBeNotifiedAboutDeletion()),
                CancellationToken.None)).ToList();
        return relationships;
    }

    private async Task NotifyRelationshipsAboutCancelledDeletion(string identityToBeDeleted, IEnumerable<Relationship> relationships)
    {
        foreach (var relationship in relationships)
        {
            await _eventBus.Publish(PeerDeletionCancelledDomainEvent.Create(relationship.GetPeerOf(identityToBeDeleted), relationship.Id, identityToBeDeleted));
        }
    }
}

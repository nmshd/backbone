using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityToBeDeleted;

public class Handler : IDomainEventHandler<IdentityToBeDeletedDomainEvent>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IEventBus _eventBus;

    public Handler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        _relationshipsRepository = relationshipsRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(IdentityToBeDeletedDomainEvent @event)
    {
        var relationships = await GetRelationshipsOf(@event.IdentityAddress);

        await NotifyRelationshipsOfPeerToBeDeleted(@event.IdentityAddress, relationships, @event.GracePeriodEndsAt);
    }

    private async Task<List<Relationship>> GetRelationshipsOf(string identityAddress)
    {
        var relationships = (await _relationshipsRepository
            .ListWithoutContent(
                Relationship.HasParticipant(identityAddress).And(Relationship.HasStatusInWhichPeerShouldBeNotifiedAboutDeletion()),
                CancellationToken.None)).ToList();
        return relationships;
    }

    private async Task NotifyRelationshipsOfPeerToBeDeleted(string identityToBeDeleted, IEnumerable<Relationship> relationships, DateTime gracePeriodEndsAt)
    {
        foreach (var relationship in relationships)
        {
            await _eventBus.Publish(PeerToBeDeletedDomainEvent.Create(relationship.GetPeerOf(identityToBeDeleted), relationship.Id, identityToBeDeleted, gracePeriodEndsAt));
        }
    }
}

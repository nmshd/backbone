using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
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

        var publishEventTasks = identitiesToBeNotified.Select(notifiedAddress => _eventBus.Publish(new PeerFeatureFlagsChangedDomainEvent
        {
            PeerAddress = @event.IdentityAddress,
            NotifiedIdentityAddress = notifiedAddress
        }));

        await Task.WhenAll(publishEventTasks);
    }

    private async Task<HashSet<string>> FindAllIdentitiesToBeNotified(FeatureFlagsOfIdentityChangedDomainEvent @event)
    {
        var identitiesToBeNotified = new HashSet<string>();

        var activeAndPendingRelationships = await
            _relationshipsRepository.FindRelationships(
                r => (r.From == @event.IdentityAddress || r.To == @event.IdentityAddress)
                     && (r.Status == RelationshipStatus.Active || r.Status == RelationshipStatus.Pending),
                CancellationToken.None);

        foreach (var relationship in activeAndPendingRelationships)
        {
            if (relationship.From == @event.IdentityAddress)
            {
                identitiesToBeNotified.Add(relationship.To);
            }
            else
            {
                identitiesToBeNotified.Add(relationship.From);
            }
        }
        
        var allocations = await _relationshipTemplatesRepository.FindRelationshipTemplateAllocations(a => a.RelationshipTemplate.CreatedBy == @event.IdentityAddress, CancellationToken.None);

        foreach (var relationshipTemplateAllocation in allocations)
        {
            identitiesToBeNotified.Add(relationshipTemplateAllocation.AllocatedBy);
        }

        return identitiesToBeNotified;
    }
}

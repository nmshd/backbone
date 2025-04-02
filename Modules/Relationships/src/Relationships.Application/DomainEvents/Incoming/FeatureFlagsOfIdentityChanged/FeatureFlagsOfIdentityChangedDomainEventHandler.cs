using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.FeatureFlagsOfIdentityChanged;

public class FeatureFlagsOfIdentityChangedDomainEventHandler : IDomainEventHandler<FeatureFlagsOfIdentityChangedDomainEvent>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IEventBus _eventBus;

    public FeatureFlagsOfIdentityChangedDomainEventHandler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IEventBus eventBus)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(FeatureFlagsOfIdentityChangedDomainEvent @event)
    {
        var allocations = await _relationshipTemplatesRepository.FindRelationshipTemplateAllocations(a => a.RelationshipTemplate.CreatedBy == @event.IdentityAddress, CancellationToken.None);

        var publishEventTasks = allocations.Select(allocation => _eventBus.Publish(new PeerFeatureFlagsChangedDomainEvent
        {
            PeerAddress = @event.IdentityAddress,
            NotifiedIdentityAddress = allocation.AllocatedBy
        }));

        await Task.WhenAll(publishEventTasks);
    }
}

using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
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
            _relationshipsRepository.FindRelationshipsAndSelect(
                r => (r.From == @event.IdentityAddress || r.To == @event.IdentityAddress)
                     && (r.Status == RelationshipStatus.Active || r.Status == RelationshipStatus.Pending),
                r => new IdentityAddressesOfRelationship(r.From, r.To),
                CancellationToken.None);

        foreach (var addressPair in activeAndPendingRelationshipAddressPairs)
        {
            if (addressPair.From == @event.IdentityAddress)
            {
                identitiesToBeNotified.Add(addressPair.To);
            }
            else
            {
                identitiesToBeNotified.Add(addressPair.From);
            }
        }

        var allocatorAddresses = await _relationshipTemplatesRepository.FindRelationshipTemplateAllocationsAndSelect(
            a => a.RelationshipTemplate.CreatedBy == @event.IdentityAddress,
            a => a.AllocatedBy, CancellationToken.None);

        identitiesToBeNotified.UnionWith(allocatorAddresses);

        return identitiesToBeNotified;
    }

    private class IdentityAddressesOfRelationship
    {
        public IdentityAddress From { get; }
        public IdentityAddress To { get; }

        public IdentityAddressesOfRelationship(IdentityAddress from, IdentityAddress to)
        {
            From = from;
            To = to;
        }
    }
}



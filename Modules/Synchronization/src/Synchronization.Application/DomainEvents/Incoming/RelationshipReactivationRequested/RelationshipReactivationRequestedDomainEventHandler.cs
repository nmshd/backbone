using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationRequested;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationRequested;

public class RelationshipReactivationRequestedDomainEventHandler : IDomainEventHandler<RelationshipReactivationRequestedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<MessageCreatedDomainEventHandler> _logger;

    public RelationshipReactivationRequestedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<MessageCreatedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(RelationshipReactivationRequestedDomainEvent integrationEvent)
    {
        await CreateExternalEvents(integrationEvent);
    }

    private async Task CreateExternalEvents(RelationshipReactivationRequestedDomainEvent @event)
    {
        try
        {
            var payload = new RelationshipReactivationRequestedExternalEvent.EventPayload { RelationshipId = @event.RelationshipId };

            var externalEvent = new RelationshipReactivationRequestedExternalEvent(@event.Peer, payload);

            await _dbContext.CreateExternalEvent(externalEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

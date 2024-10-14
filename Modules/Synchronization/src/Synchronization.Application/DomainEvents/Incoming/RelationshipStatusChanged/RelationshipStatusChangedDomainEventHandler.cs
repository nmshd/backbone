using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipStatusChanged;

public class RelationshipStatusChangedDomainEventHandler : IDomainEventHandler<RelationshipStatusChangedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<RelationshipStatusChangedDomainEventHandler> _logger;

    public RelationshipStatusChangedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<RelationshipStatusChangedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(RelationshipStatusChangedDomainEvent @event)
    {
        // if the relationship is in status "ReadyForDeletion", the peer doesn't know anything about it; therefore we must not create an external event
        if (@event.NewStatus == "ReadyForDeletion")
            return;

        try
        {
            var payload = new RelationshipStatusChangedExternalEvent.EventPayload { RelationshipId = @event.RelationshipId };

            var externalEvent = new RelationshipStatusChangedExternalEvent(@event.Peer, payload);

            await _dbContext.CreateExternalEvent(externalEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

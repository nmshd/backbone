using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationCompleted;

public class RelationshipReactivationCompletedDomainEventHandler : IDomainEventHandler<RelationshipReactivationCompletedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<RelationshipReactivationCompletedDomainEventHandler> _logger;

    public RelationshipReactivationCompletedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<RelationshipReactivationCompletedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(RelationshipReactivationCompletedDomainEvent @event)
    {
        try
        {
            await CreateRelationshipReactivationCompletedExternalEvent(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }

    private async Task CreateRelationshipReactivationCompletedExternalEvent(RelationshipReactivationCompletedDomainEvent @event)
    {
        var payload = new RelationshipReactivationCompletedExternalEvent.EventPayload { RelationshipId = @event.RelationshipId };

        var externalEvent = new RelationshipReactivationCompletedExternalEvent(@event.Peer, payload);

        await _dbContext.CreateExternalEvent(externalEvent);
    }
}

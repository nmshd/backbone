using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipCreated;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipCreated;

public class RelationshipCreatedDomainEventHandler : IDomainEventHandler<RelationshipCreatedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<RelationshipCreatedDomainEventHandler> _logger;

    public RelationshipCreatedDomainEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<RelationshipCreatedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(RelationshipCreatedDomainEvent @event)
    {
#pragma warning disable IDE0037
        var payload = new { RelationshipId = @event.RelationshipId };
#pragma warning restore IDE0037
        try
        {
            var externalEvent = await _dbContext.CreateExternalEvent(@event.To, ExternalEventType.RelationshipCreated, payload);
            _eventBus.Publish(new ExternalEventCreatedDomainEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing an Domain event.");
            throw;
        }
    }
}

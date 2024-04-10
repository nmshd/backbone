using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipCreated;

public class RelationshipCreatedIntegrationEventHandler : IIntegrationEventHandler<RelationshipCreatedIntegrationEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<RelationshipCreatedIntegrationEventHandler> _logger;

    public RelationshipCreatedIntegrationEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<RelationshipCreatedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(RelationshipCreatedIntegrationEvent @event)
    {
        try
        {
            // ReSharper disable once RedundantAnonymousTypePropertyName
            var payload = new { @event.RelationshipId };
            var externalEvent = await _dbContext.CreateExternalEvent(@event.To, ExternalEventType.RelationshipCreated, payload);
            _eventBus.Publish(new ExternalEventCreatedIntegrationEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing an integration event.");
            throw;
        }
    }
}

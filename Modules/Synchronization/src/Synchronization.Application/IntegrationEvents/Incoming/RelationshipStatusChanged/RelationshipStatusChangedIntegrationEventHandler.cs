using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipStatusChanged;

public class RelationshipStatusChangedIntegrationEventHandler : IIntegrationEventHandler<RelationshipStatusChangedIntegrationEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<RelationshipStatusChangedIntegrationEventHandler> _logger;

    public RelationshipStatusChangedIntegrationEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<RelationshipStatusChangedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(RelationshipStatusChangedIntegrationEvent @event)
    {
        try
        {
            // ReSharper disable once RedundantAnonymousTypePropertyName
            var payload = new { RelationshipId = @event.RelationshipId };
            var externalEvent = await _dbContext.CreateExternalEvent(@event.Peer, ExternalEventType.RelationshipStatusChanged, payload);
            _eventBus.Publish(new ExternalEventCreatedIntegrationEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing an integration event.");
            throw;
        }
    }
}

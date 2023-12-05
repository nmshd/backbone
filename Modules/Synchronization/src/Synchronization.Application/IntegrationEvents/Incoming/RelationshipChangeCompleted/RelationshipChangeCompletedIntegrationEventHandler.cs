using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipChangeCompleted;

public class RelationshipChangeCompletedIntegrationEventHandler : IIntegrationEventHandler<RelationshipChangeCompletedIntegrationEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<RelationshipChangeCompletedIntegrationEventHandler> _logger;

    public RelationshipChangeCompletedIntegrationEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<RelationshipChangeCompletedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(RelationshipChangeCompletedIntegrationEvent integrationEvent)
    {
        await CreateExternalEvent(integrationEvent);
    }

    private async Task CreateExternalEvent(RelationshipChangeCompletedIntegrationEvent integrationEvent)
    {
        var payload = new { RelationshipId = integrationEvent.RelationshipId, ChangeId = integrationEvent.ChangeId };
        try
        {
            var owner = integrationEvent.ChangeResult switch
            {
                "Accepted" => integrationEvent.ChangeCreatedBy,
                "Rejected" => integrationEvent.ChangeCreatedBy,
                "Revoked" => integrationEvent.ChangeRecipient,
                _ => throw new ArgumentOutOfRangeException(nameof(integrationEvent.ChangeResult), integrationEvent, null)
            };

            var externalEvent = await _dbContext.CreateExternalEvent(owner, ExternalEventType.RelationshipChangeCompleted, payload);
            _eventBus.Publish(new ExternalEventCreatedIntegrationEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing an integration event.");
            throw;
        }
    }
}

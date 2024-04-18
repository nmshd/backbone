using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipReactivationCompleted;
public class RelationshipReactivationCompletedIntegrationEventHandler : IIntegrationEventHandler<RelationshipReactivationCompletedIntegrationEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<MessageCreatedIntegrationEventHandler> _logger;

    public RelationshipReactivationCompletedIntegrationEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<MessageCreatedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(RelationshipReactivationCompletedIntegrationEvent integrationEvent)
    {
        await CreateExternalEvent(integrationEvent);
    }

    private async Task CreateExternalEvent(RelationshipReactivationCompletedIntegrationEvent @event)
    {
#pragma warning disable IDE0037
        var payload = new { RelationshipId = @event.RelationshipId };
#pragma warning restore IDE0037
        try
        {
            var externalEvent = await _dbContext.CreateExternalEvent(
                IdentityAddress.Parse(@event.Peer), ExternalEventType.RelationshipReactivationCompleted, payload);
            _eventBus.Publish(new ExternalEventCreatedIntegrationEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing an integration event.");
            throw;
        }
    }
}

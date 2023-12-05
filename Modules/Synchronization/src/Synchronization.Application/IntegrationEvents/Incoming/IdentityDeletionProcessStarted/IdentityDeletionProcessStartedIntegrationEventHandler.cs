using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStarted;

public class IdentityDeletionProcessStartedIntegrationEventHandler : IIntegrationEventHandler<IdentityDeletionProcessStartedIntegrationEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<IdentityDeletionProcessStartedIntegrationEventHandler> _logger;

    public IdentityDeletionProcessStartedIntegrationEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<IdentityDeletionProcessStartedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(IdentityDeletionProcessStartedIntegrationEvent integrationEvent)
    {
        var payload = new { DeletionProcessId = integrationEvent.DeletionProcessId };
        try
        {
            var externalEvent = await _dbContext.CreateExternalEvent(IdentityAddress.Parse(integrationEvent.Address), ExternalEventType.IdentityDeletionProcessStarted, payload);
            _eventBus.Publish(new ExternalEventCreatedIntegrationEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing an integration event.");
            throw;
        }
    }
}

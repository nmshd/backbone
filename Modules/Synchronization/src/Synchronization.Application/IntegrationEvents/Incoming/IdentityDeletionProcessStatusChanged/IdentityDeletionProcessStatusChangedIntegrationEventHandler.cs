using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStatusChanged;
public class IdentityDeletionProcessStatusChangedIntegrationEventHandler : IIntegrationEventHandler<IdentityDeletionProcessStatusChangedIntegrationEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<IdentityDeletionProcessStatusChangedIntegrationEventHandler> _logger;

    public IdentityDeletionProcessStatusChangedIntegrationEventHandler(ISynchronizationDbContext dbContext,
        IEventBus eventBus,
        ILogger<IdentityDeletionProcessStatusChangedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(IdentityDeletionProcessStatusChangedIntegrationEvent @event)
    {
#pragma warning disable IDE0037
        var payload = new { DeletionProcessId = @event.DeletionProcessId };
#pragma warning restore IDE0037
        try
        {
            var externalEvent = await _dbContext.CreateExternalEvent(IdentityAddress.Parse(@event.Address), ExternalEventType.IdentityDeletionProcessStatusChanged, payload);
            _eventBus.Publish(new ExternalEventCreatedIntegrationEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing an integration event.");
            throw;
        }
    }
}

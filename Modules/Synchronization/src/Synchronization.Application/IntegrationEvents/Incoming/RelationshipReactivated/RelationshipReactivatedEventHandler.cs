﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipReactivated;
public class RelationshipReactivatedEventHandler
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<MessageCreatedIntegrationEventHandler> _logger;

    public RelationshipReactivatedEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<MessageCreatedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(RelationshipReactivatedIntegrationEvent integrationEvent)
    {
        await CreateExternalEvents(integrationEvent);
    }

    private async Task CreateExternalEvents(RelationshipReactivatedIntegrationEvent @event)
    {
#pragma warning disable IDE0037
        var payload = new { Id = @event.RelationshipId };
#pragma warning restore IDE0037
        try
        {
            var externalEvent = await _dbContext.CreateExternalEvent(IdentityAddress.Parse(@event.Partner), ExternalEventType.RelationshipReactivationRequested, payload);
            _eventBus.Publish(new ExternalEventCreatedIntegrationEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing an integration event.");
            throw;
        }
    }
}

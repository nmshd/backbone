﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationCompleted;
public class RelationshipReactivationCompletedDomainEventHandler
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<MessageCreatedDomainEventHandler> _logger;

    public RelationshipReactivationCompletedDomainEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<MessageCreatedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(RelationshipReactivationCompletedDomainEvent domainEvent)
    {
        await CreateExternalEvent(domainEvent);
    }

    private async Task CreateExternalEvent(RelationshipReactivationCompletedDomainEvent @event)
    {
#pragma warning disable IDE0037
        var payload = new { @event.RelationshipId };
#pragma warning restore IDE0037
        try
        {
            var externalEvent = await _dbContext.CreateExternalEvent(
                IdentityAddress.Parse(@event.Peer), ExternalEventType.RelationshipReactivationCompleted, payload);
            _eventBus.Publish(new ExternalEventCreatedDomainEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

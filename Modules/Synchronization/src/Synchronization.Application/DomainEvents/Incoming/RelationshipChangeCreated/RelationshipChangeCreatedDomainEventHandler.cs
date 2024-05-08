using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipChangeCreated;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipChangeCreated;

public class RelationshipChangeCreatedDomainEventHandler : IDomainEventHandler<RelationshipChangeCreatedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<RelationshipChangeCreatedDomainEventHandler> _logger;

    public RelationshipChangeCreatedDomainEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<RelationshipChangeCreatedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(RelationshipChangeCreatedDomainEvent domainEvent)
    {
        await CreateExternalEvent(domainEvent);
    }

    private async Task CreateExternalEvent(RelationshipChangeCreatedDomainEvent domainEvent)
    {
#pragma warning disable IDE0037
        var payload = new { RelationshipId = domainEvent.RelationshipId, ChangeId = domainEvent.ChangeId };
#pragma warning restore IDE0037
        try
        {
            var externalEvent = await _dbContext.CreateExternalEvent(domainEvent.ChangeRecipient, ExternalEventType.RelationshipChangeCreated, payload);
            _eventBus.Publish(new ExternalEventCreatedDomainEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing an domain event.");
            throw;
        }
    }
}

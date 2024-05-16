using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipChangeCreated;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipChangeCreated;

public class RelationshipChangeCreatedDomainEventHandler : IDomainEventHandler<RelationshipChangeCreatedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<RelationshipChangeCreatedDomainEventHandler> _logger;

    public RelationshipChangeCreatedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<RelationshipChangeCreatedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
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
            await _dbContext.CreateExternalEvent(domainEvent.ChangeRecipient, ExternalEventType.RelationshipChangeCreated, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing an domain event.");
            throw;
        }
    }
}

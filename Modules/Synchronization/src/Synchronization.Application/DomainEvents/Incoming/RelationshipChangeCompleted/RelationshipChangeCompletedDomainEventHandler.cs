using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipChangeCompleted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipChangeCompleted;

public class RelationshipChangeCompletedDomainEventHandler : IDomainEventHandler<RelationshipChangeCompletedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<RelationshipChangeCompletedDomainEventHandler> _logger;

    public RelationshipChangeCompletedDomainEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<RelationshipChangeCompletedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(RelationshipChangeCompletedDomainEvent domainEvent)
    {
        await CreateExternalEvent(domainEvent);
    }

    private async Task CreateExternalEvent(RelationshipChangeCompletedDomainEvent domainEvent)
    {
#pragma warning disable IDE0037
        var payload = new { RelationshipId = domainEvent.RelationshipId, ChangeId = domainEvent.ChangeId };
#pragma warning restore IDE0037
        try
        {
            var owner = domainEvent.ChangeResult switch
            {
                "Accepted" => domainEvent.ChangeCreatedBy,
                "Rejected" => domainEvent.ChangeCreatedBy,
                "Revoked" => domainEvent.ChangeRecipient,
                _ => throw new ArgumentOutOfRangeException(nameof(domainEvent.ChangeResult), domainEvent, null)
            };

            var externalEvent = await _dbContext.CreateExternalEvent(owner, ExternalEventType.RelationshipChangeCompleted, payload);
            _eventBus.Publish(new ExternalEventCreatedDomainEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing an domain event.");
            throw;
        }
    }
}

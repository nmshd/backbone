using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipStatusChanged;

public class RelationshipStatusChangedDomainEventHandler : IDomainEventHandler<RelationshipStatusChangedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<RelationshipStatusChangedDomainEventHandler> _logger;

    public RelationshipStatusChangedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<RelationshipStatusChangedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(RelationshipStatusChangedDomainEvent @event)
    {
        try
        {
            await CreateRelationshipStatusChangedExternalEvent(@event);
            await DeleteExternalEvents(@event);
            await UnblockMessageReceivedExternalEvents(@event);
            await DeleteBlockedMessageReceivedExternalEvents(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }

    private async Task CreateRelationshipStatusChangedExternalEvent(RelationshipStatusChangedDomainEvent @event)
    {
        // if the relationship is in status "ReadyForDeletion", the peer doesn't know anything about it; therefore we must not create an external event
        if (@event.NewStatus == "ReadyForDeletion")
            return;

        var payload = new RelationshipStatusChangedExternalEvent.EventPayload { RelationshipId = @event.RelationshipId };

        var externalEvent = new RelationshipStatusChangedExternalEvent(@event.Peer, payload);

        await _dbContext.CreateExternalEvent(externalEvent);
    }

    private async Task DeleteExternalEvents(RelationshipStatusChangedDomainEvent @event)
    {
        // when a relationship was decomposed, we can delete all external events related to it for
        // the identity that initiated the decomposition
        if (@event.NewStatus is "DeletionProposed" or "ReadyForDeletion")
        {
            await _dbContext.DeleteUnsyncedExternalEventsWithOwnerAndContext(@event.Initiator, @event.RelationshipId);
        }
    }

    private async Task UnblockMessageReceivedExternalEvents(RelationshipStatusChangedDomainEvent @event)
    {
        if (@event.NewStatus != "Active")
            return;

        var externalEvents = await _dbContext.GetBlockedExternalEventsWithTypeAndContext(ExternalEventType.MessageReceived, @event.RelationshipId, CancellationToken.None);

        foreach (var externalEvent in externalEvents)
        {
            externalEvent.UnblockDelivery();
            _dbContext.Set<ExternalEvent>().Entry(externalEvent).CurrentValues.SetValues(externalEvent);
        }

        await _dbContext.SaveChangesAsync(CancellationToken.None);
    }

    private async Task DeleteBlockedMessageReceivedExternalEvents(RelationshipStatusChangedDomainEvent @event)
    {
        if (@event.NewStatus is not "Revoked" and not "Rejected")
            return;

        await _dbContext.DeleteBlockedExternalEventsWithTypeAndContext(ExternalEventType.MessageReceived, @event.RelationshipId, CancellationToken.None);
    }
}

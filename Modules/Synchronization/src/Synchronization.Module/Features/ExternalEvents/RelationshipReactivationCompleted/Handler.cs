using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Module.Features.ExternalEvents.RelationshipReactivationCompleted;

public class Handler : IDomainEventHandler<RelationshipReactivationCompletedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<Handler> _logger;

    public Handler(ISynchronizationDbContext dbContext, ILogger<Handler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(RelationshipReactivationCompletedDomainEvent @event)
    {
        try
        {
            await CreateRelationshipReactivationCompletedExternalEvent(@event);
            await UnblockMessageReceivedExternalEvents(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }

    private async Task CreateRelationshipReactivationCompletedExternalEvent(RelationshipReactivationCompletedDomainEvent @event)
    {
        var payload = new RelationshipReactivationCompletedExternalEvent.EventPayload { RelationshipId = @event.RelationshipId };

        var externalEvent = new RelationshipReactivationCompletedExternalEvent(@event.Peer, payload);

        await _dbContext.CreateExternalEvent(externalEvent);
    }

    private async Task UnblockMessageReceivedExternalEvents(RelationshipReactivationCompletedDomainEvent @event)
    {
        if (@event.NewRelationshipStatus != "Active")
            return;

        var externalEvents = await _dbContext.GetBlockedExternalEventsWithTypeAndContext(ExternalEventType.MessageReceived, @event.RelationshipId, CancellationToken.None);

        foreach (var externalEvent in externalEvents)
        {
            externalEvent.UnblockDelivery();
        }

        await _dbContext.SaveChangesAsync(CancellationToken.None);
    }
}

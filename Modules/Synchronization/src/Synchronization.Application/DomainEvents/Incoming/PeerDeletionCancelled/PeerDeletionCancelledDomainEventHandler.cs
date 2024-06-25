using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeletionCancelled;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeletionCancelled;

public class PeerDeletionCancelledDomainEventHandler : IDomainEventHandler<PeerDeletionCancelledDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<PeerDeletionCancelledDomainEventHandler> _logger;

    public PeerDeletionCancelledDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<PeerDeletionCancelledDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(PeerDeletionCancelledDomainEvent domainEvent)
    {
        await CreateExternalEvent(domainEvent);
    }

    private async Task CreateExternalEvent(PeerDeletionCancelledDomainEvent @event)
    {
#pragma warning disable IDE0037
        var payload = new { RelationshipId = @event.RelationshipId };
#pragma warning restore IDE0037
        try
        {
            await _dbContext.CreateExternalEvent(@event.PeerOfIdentityWithDeletionCancelled, ExternalEventType.PeerDeletionCancelled, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

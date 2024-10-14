using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
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
        await CreatePeerDeletionCancelledExternalEvent(domainEvent);
    }

    private async Task CreatePeerDeletionCancelledExternalEvent(PeerDeletionCancelledDomainEvent @event)
    {
        try
        {
            var payload = new PeerDeletionCancelledExternalEvent.EventPayload { RelationshipId = @event.RelationshipId };

            var externalEvent = new PeerDeletionCancelledExternalEvent(IdentityAddress.Parse(@event.PeerOfIdentityWithDeletionCancelled), payload);

            await _dbContext.CreateExternalEvent(externalEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

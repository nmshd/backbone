using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeleted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Tooling;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeleted;

public class PeerDeletedDomainEventHandler : IDomainEventHandler<PeerDeletedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<PeerDeletedDomainEventHandler> _logger;

    public PeerDeletedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<PeerDeletedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(PeerDeletedDomainEvent domainEvent)
    {
        await CreatePeerDeletedExternalEvent(domainEvent);
    }

    private async Task CreatePeerDeletedExternalEvent(PeerDeletedDomainEvent @event)
    {
        try
        {
            var payload = new PeerDeletedExternalEvent.EventPayload { RelationshipId = @event.RelationshipId, DeletionDate = SystemTime.UtcNow };

            var externalEvent = new PeerDeletedExternalEvent(IdentityAddress.Parse(@event.PeerOfDeletedIdentity), payload);

            await _dbContext.CreateExternalEvent(externalEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

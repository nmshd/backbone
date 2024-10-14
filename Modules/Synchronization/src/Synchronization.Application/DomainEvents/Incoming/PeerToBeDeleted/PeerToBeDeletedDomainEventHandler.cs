using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerToBeDeleted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerToBeDeleted;

public class PeerToBeDeletedDomainEventHandler : IDomainEventHandler<PeerToBeDeletedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<PeerToBeDeletedDomainEventHandler> _logger;

    public PeerToBeDeletedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<PeerToBeDeletedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(PeerToBeDeletedDomainEvent domainEvent)
    {
        await CreatePeerToBeDeletedExternalEvent(domainEvent);
    }

    private async Task CreatePeerToBeDeletedExternalEvent(PeerToBeDeletedDomainEvent @event)
    {
        try
        {
            var payload = new PeerToBeDeletedExternalEvent.EventPayload { RelationshipId = @event.RelationshipId, DeletionDate = @event.GracePeriodEndsAt };

            var externalEvent = new PeerToBeDeletedExternalEvent(IdentityAddress.Parse(@event.PeerOfIdentityToBeDeleted), payload);

            await _dbContext.CreateExternalEvent(externalEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

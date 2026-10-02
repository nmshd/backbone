using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerToBeDeleted;

public class Handler : IDomainEventHandler<PeerToBeDeletedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<Handler> _logger;

    public Handler(ISynchronizationDbContext dbContext, ILogger<Handler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(PeerToBeDeletedDomainEvent @event)
    {
        try
        {
            await CreatePeerToBeDeletedExternalEvent(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }

    private async Task CreatePeerToBeDeletedExternalEvent(PeerToBeDeletedDomainEvent @event)
    {
        var payload = new PeerToBeDeletedExternalEvent.EventPayload { RelationshipId = @event.RelationshipId, DeletionDate = @event.GracePeriodEndsAt };

        var externalEvent = new PeerToBeDeletedExternalEvent(IdentityAddress.Parse(@event.PeerOfIdentityToBeDeleted), payload);

        await _dbContext.CreateExternalEvent(externalEvent);
    }
}

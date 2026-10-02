using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerDeletionCancelled;

public class Handler : IDomainEventHandler<PeerDeletionCancelledDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<Handler> _logger;

    public Handler(ISynchronizationDbContext dbContext, ILogger<Handler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(PeerDeletionCancelledDomainEvent @event)
    {
        try
        {
            await CreatePeerDeletionCancelledExternalEvent(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }

    private async Task CreatePeerDeletionCancelledExternalEvent(PeerDeletionCancelledDomainEvent @event)
    {
        var payload = new PeerDeletionCancelledExternalEvent.EventPayload { RelationshipId = @event.RelationshipId };

        var externalEvent = new PeerDeletionCancelledExternalEvent(IdentityAddress.Parse(@event.PeerOfIdentityWithDeletionCancelled), payload);

        await _dbContext.CreateExternalEvent(externalEvent);
    }
}

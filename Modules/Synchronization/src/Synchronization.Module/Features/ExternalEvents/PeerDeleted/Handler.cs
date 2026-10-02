using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Tooling;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerDeleted;

public class Handler : IDomainEventHandler<PeerDeletedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<Handler> _logger;

    public Handler(ISynchronizationDbContext dbContext, ILogger<Handler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(PeerDeletedDomainEvent @event)
    {
        try
        {
            await CreatePeerDeletedExternalEvent(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }

    private async Task CreatePeerDeletedExternalEvent(PeerDeletedDomainEvent @event)
    {
        var payload = new PeerDeletedExternalEvent.EventPayload { RelationshipId = @event.RelationshipId, DeletionDate = SystemTime.UtcNow };

        var externalEvent = new PeerDeletedExternalEvent(IdentityAddress.Parse(@event.PeerOfDeletedIdentity), payload);

        await _dbContext.CreateExternalEvent(externalEvent);
    }
}

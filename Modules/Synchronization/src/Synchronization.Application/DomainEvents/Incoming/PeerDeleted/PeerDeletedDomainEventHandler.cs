using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeleted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
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
        await CreateExternalEvent(domainEvent);
    }

    private async Task CreateExternalEvent(PeerDeletedDomainEvent @event)
    {
#pragma warning disable IDE0037
        var payload = new
        {
            RelationshipId = @event.RelationshipId,
            DeletionDate = DateTime.UtcNow
        };
#pragma warning restore IDE0037
        try
        {
            await _dbContext.CreateExternalEvent(@event.PeerOfDeletedIdentity, ExternalEventType.PeerDeleted, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

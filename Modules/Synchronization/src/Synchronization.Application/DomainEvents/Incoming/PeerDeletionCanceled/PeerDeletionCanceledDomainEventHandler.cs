using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeletionCanceled;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeletionCanceled;
public class PeerDeletionCanceledDomainEventHandler : IDomainEventHandler<PeerDeletionCanceledDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<PeerDeletionCanceledDomainEventHandler> _logger;

    public PeerDeletionCanceledDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<PeerDeletionCanceledDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(PeerDeletionCanceledDomainEvent domainEvent)
    {
        await CreateExternalEvent(domainEvent);
    }

    private async Task CreateExternalEvent(PeerDeletionCanceledDomainEvent @event)
    {
#pragma warning disable IDE0037
        var payload = new { RelationshipId = @event.RelationshipId };
#pragma warning restore IDE0037
        try
        {
            await _dbContext.CreateExternalEvent(@event.PeerOfIdentityWithDeletionCanceled, ExternalEventType.PeerDeletionCanceled, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

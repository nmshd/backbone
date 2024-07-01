using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipDecomposed;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipDecomposed;

public class RelationshipDecomposedDomainEventHandler : IDomainEventHandler<RelationshipDecomposedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<RelationshipDecomposedDomainEventHandler> _logger;

    public RelationshipDecomposedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<RelationshipDecomposedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(RelationshipDecomposedDomainEvent integrationEvent)
    {
        await CreateExternalEvents(integrationEvent);
    }

    private async Task CreateExternalEvents(RelationshipDecomposedDomainEvent @event)
    {
#pragma warning disable IDE0037
        var payload = new { RelationshipId = @event.RelationshipId };
#pragma warning restore IDE0037
        try
        {
            await _dbContext.CreateExternalEvent(IdentityAddress.Parse(@event.Peer), ExternalEventType.RelationshipDecomposedByPeer, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing an integration event.");
            throw;
        }
    }
}

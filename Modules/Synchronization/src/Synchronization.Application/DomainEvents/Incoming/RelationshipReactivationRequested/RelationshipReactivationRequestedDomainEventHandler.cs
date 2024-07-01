using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationRequested;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationRequested;

public class RelationshipReactivationRequestedDomainEventHandler : IDomainEventHandler<RelationshipReactivationRequestedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<MessageCreatedDomainEventHandler> _logger;

    public RelationshipReactivationRequestedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<MessageCreatedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(RelationshipReactivationRequestedDomainEvent integrationEvent)
    {
        await CreateExternalEvents(integrationEvent);
    }

    private async Task CreateExternalEvents(RelationshipReactivationRequestedDomainEvent @event)
    {
#pragma warning disable IDE0037
        var payload = new { RelationshipId = @event.RelationshipId };
#pragma warning restore IDE0037
        try
        {
            await _dbContext.CreateExternalEvent(IdentityAddress.Parse(@event.Peer), ExternalEventType.RelationshipReactivationRequested, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

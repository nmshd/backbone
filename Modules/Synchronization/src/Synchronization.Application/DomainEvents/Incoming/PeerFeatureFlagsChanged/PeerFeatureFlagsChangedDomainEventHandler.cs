using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerFeatureFlagsChangedEvent;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerFeatureFlagsChanged;

public class PeerFeatureFlagsChangedDomainEventHandler : IDomainEventHandler<PeerFeatureFlagsChangedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<PeerFeatureFlagsChangedDomainEventHandler> _logger;

    public PeerFeatureFlagsChangedDomainEventHandler(ILogger<PeerFeatureFlagsChangedDomainEventHandler> logger, ISynchronizationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(PeerFeatureFlagsChangedDomainEvent @event)
    {
        try
        {
            await CreatePeerFeatureFlagsChangedExternalEvent(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }

    private async Task CreatePeerFeatureFlagsChangedExternalEvent(PeerFeatureFlagsChangedDomainEvent @event)
    {
        var payload = new PeerFeatureFlagsChangedExternalEvent.EventPayload { PeerAddress = @event.PeerAddress };

        var externalEvent = new PeerFeatureFlagsChangedExternalEvent(IdentityAddress.Parse(@event.NotifiedIdentityAddress), payload);

        await _dbContext.CreateExternalEvent(externalEvent);
    }
}

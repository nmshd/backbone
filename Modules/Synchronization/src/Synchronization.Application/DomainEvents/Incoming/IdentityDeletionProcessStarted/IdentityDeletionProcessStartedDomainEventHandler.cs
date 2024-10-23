using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;

public class IdentityDeletionProcessStartedDomainEventHandler : IDomainEventHandler<IdentityDeletionProcessStartedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<IdentityDeletionProcessStartedDomainEventHandler> _logger;

    public IdentityDeletionProcessStartedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<IdentityDeletionProcessStartedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(IdentityDeletionProcessStartedDomainEvent @event)
    {
        try
        {
            await CreateIdentityDeletionProcessStartedExternalEvent(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }

    private async Task CreateIdentityDeletionProcessStartedExternalEvent(IdentityDeletionProcessStartedDomainEvent @event)
    {
        // No need to create an external event if the deletion process was started by the identity itself (in that case it's not "external").
        if (@event.Initiator == @event.Address)
            return;

        var payload = new IdentityDeletionProcessStartedExternalEvent.EventPayload { DeletionProcessId = @event.DeletionProcessId };

        var externalEvent = new IdentityDeletionProcessStartedExternalEvent(IdentityAddress.Parse(@event.Address), payload);

        await _dbContext.CreateExternalEvent(externalEvent);
    }
}

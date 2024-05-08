using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;

public class IdentityDeletionProcessStartedDomainEventHandler : IDomainEventHandler<IdentityDeletionProcessStartedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<IdentityDeletionProcessStartedDomainEventHandler> _logger;

    public IdentityDeletionProcessStartedDomainEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<IdentityDeletionProcessStartedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(IdentityDeletionProcessStartedDomainEvent domainEvent)
    {
        // No need to create an external event if the deletion process was started by the identity itself (in that case it's not "external").
        if (domainEvent.Initiator == domainEvent.Address)
            return;

#pragma warning disable IDE0037
        var payload = new { DeletionProcessId = domainEvent.DeletionProcessId };
#pragma warning restore IDE0037
        try
        {
            var externalEvent = await _dbContext.CreateExternalEvent(IdentityAddress.Parse(domainEvent.Address), ExternalEventType.IdentityDeletionProcessStarted, payload);
            _eventBus.Publish(new ExternalEventCreatedDomainEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing an domain event.");
            throw;
        }
    }
}

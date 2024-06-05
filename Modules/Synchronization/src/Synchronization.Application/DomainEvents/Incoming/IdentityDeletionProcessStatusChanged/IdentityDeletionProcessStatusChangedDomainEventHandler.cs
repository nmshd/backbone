using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;

public class IdentityDeletionProcessStatusChangedDomainEventHandler : IDomainEventHandler<IdentityDeletionProcessStatusChangedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<IdentityDeletionProcessStatusChangedDomainEventHandler> _logger;

    public IdentityDeletionProcessStatusChangedDomainEventHandler(ISynchronizationDbContext dbContext,
        ILogger<IdentityDeletionProcessStatusChangedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(IdentityDeletionProcessStatusChangedDomainEvent @event)
    {
        // No need to create an external event if the action that triggered the event was initiated by the owner of the deletion process (in that case it's not "external").
        if (@event.Initiator == @event.Address)
            return;

#pragma warning disable IDE0037
        var payload = new { DeletionProcessId = @event.DeletionProcessId };
#pragma warning restore IDE0037
        try
        {
            await _dbContext.CreateExternalEvent(IdentityAddress.Parse(@event.Address), ExternalEventType.IdentityDeletionProcessStatusChanged, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing an domain event.");
            throw;
        }
    }
}

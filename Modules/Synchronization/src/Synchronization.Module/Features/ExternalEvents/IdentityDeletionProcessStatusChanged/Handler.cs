using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Module.Features.ExternalEvents.IdentityDeletionProcessStatusChanged;

public class Handler : IDomainEventHandler<IdentityDeletionProcessStatusChangedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<Handler> _logger;

    public Handler(ISynchronizationDbContext dbContext,
        ILogger<Handler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(IdentityDeletionProcessStatusChangedDomainEvent @event)
    {
        try
        {
            await CreateIdentityDeletionProcessStatusChangedExternalEvent(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }

    private async Task CreateIdentityDeletionProcessStatusChangedExternalEvent(IdentityDeletionProcessStatusChangedDomainEvent @event)
    {
        // No need to create an external event if the action that triggered the event was initiated by the owner of the deletion process (in that case it's not "external").
        if (@event.Initiator == @event.DeletionProcessOwner)
            return;

        var payload = new IdentityDeletionProcessStatusChangedExternalEvent.EventPayload { DeletionProcessId = @event.DeletionProcessId };

        var externalEvent = new IdentityDeletionProcessStatusChangedExternalEvent(IdentityAddress.Parse(@event.DeletionProcessOwner), payload);

        await _dbContext.CreateExternalEvent(externalEvent);
    }
}

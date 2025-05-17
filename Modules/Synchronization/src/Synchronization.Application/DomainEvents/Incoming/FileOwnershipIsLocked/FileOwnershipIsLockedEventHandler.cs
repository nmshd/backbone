using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.FileOwnershipIsLockedEvent;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.FileOwnershipIsLocked;

public class FileOwnershipIsLockedEventHandler : IDomainEventHandler<FileOwnershipIsLockedEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<FileOwnershipIsLockedEventHandler> _logger;

    public FileOwnershipIsLockedEventHandler(ILogger<FileOwnershipIsLockedEventHandler> logger, ISynchronizationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }


    public async Task Handle(FileOwnershipIsLockedEvent @event)
    {
        try
        {
            var payload = new FileOwnershipIsLockedExternalEvent.EventPayload { FileAddress = @event.FileAddress };

            var externalEvent = new FileOwnershipIsLockedExternalEvent(IdentityAddress.Parse(@event.OwnerAddress), payload);

            await _dbContext.CreateExternalEvent(externalEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

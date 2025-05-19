using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.FileOwnershipIsLockedEvent;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.FileOwnershipLocked;

public class FileOwnershipLockedEventHandler : IDomainEventHandler<FileOwnershipLockedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<FileOwnershipLockedEventHandler> _logger;

    public FileOwnershipLockedEventHandler(ILogger<FileOwnershipLockedEventHandler> logger, ISynchronizationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }


    public async Task Handle(FileOwnershipLockedDomainEvent domainEvent)
    {
        try
        {
            var payload = new FileOwnershipLockedExternalEvent.EventPayload { FileId = domainEvent.FileId };

            var externalEvent = new FileOwnershipLockedExternalEvent(IdentityAddress.Parse(domainEvent.OwnerAddress), payload);

            await _dbContext.CreateExternalEvent(externalEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

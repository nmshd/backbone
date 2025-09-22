using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.FileOwnershipIsLocked;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.FileOwnershipLocked;

public class FileOwnershipLockedDomainEventHandler : IDomainEventHandler<FileOwnershipLockedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<FileOwnershipLockedDomainEventHandler> _logger;

    public FileOwnershipLockedDomainEventHandler(ILogger<FileOwnershipLockedDomainEventHandler> logger, ISynchronizationDbContext dbContext)
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

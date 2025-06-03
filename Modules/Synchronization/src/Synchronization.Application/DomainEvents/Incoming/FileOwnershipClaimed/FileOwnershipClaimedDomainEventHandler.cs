using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.FileOwnershipClaimed;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.FileOwnershipClaimed;

public class FileOwnershipClaimedDomainEventHandler : IDomainEventHandler<FileOwnershipClaimedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<FileOwnershipClaimedDomainEventHandler> _logger;

    public FileOwnershipClaimedDomainEventHandler(ILogger<FileOwnershipClaimedDomainEventHandler> logger, ISynchronizationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(FileOwnershipClaimedDomainEvent domainEvent)
    {
        try
        {
            var payload = new FileOwnershipClaimedExternalEvent.EventPayload { FileId = domainEvent.FileId, NewOwnerAddress = domainEvent.NewOwnerAddress };

            var externalEvent = new FileOwnershipClaimedExternalEvent(IdentityAddress.Parse(domainEvent.OldOwnerAddress), payload);

            await _dbContext.CreateExternalEvent(externalEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

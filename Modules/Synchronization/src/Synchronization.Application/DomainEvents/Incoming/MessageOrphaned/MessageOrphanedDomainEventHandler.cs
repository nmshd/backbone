using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.MessageOrphaned;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.MessageOrphaned;

public class MessageOrphanedDomainEventHandler : IDomainEventHandler<MessageOrphanedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<MessageOrphanedDomainEventHandler> _logger;

    public MessageOrphanedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<MessageOrphanedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(MessageOrphanedDomainEvent @event)
    {
#pragma warning disable IDE0037
        var payload = new { IdentityAddress = @event.IdentityAddress };
#pragma warning restore IDE0037
        try
        {
            await _dbContext.CreateExternalEvent(IdentityAddress.Parse(@event.IdentityAddress), ExternalEventType.MessageOrphaned, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}

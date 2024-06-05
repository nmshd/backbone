using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.MessageCreated;

public class MessageCreatedDomainEventHandler : IDomainEventHandler<MessageCreatedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly ILogger<MessageCreatedDomainEventHandler> _logger;

    public MessageCreatedDomainEventHandler(ISynchronizationDbContext dbContext, ILogger<MessageCreatedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(MessageCreatedDomainEvent domainEvent)
    {
        await CreateExternalEvents(domainEvent);
    }

    private async Task CreateExternalEvents(MessageCreatedDomainEvent domainEvent)
    {
        foreach (var recipient in domainEvent.Recipients)
        {
#pragma warning disable IDE0037
            var payload = new { Id = domainEvent.Id };
#pragma warning restore IDE0037
            try
            {
                await _dbContext.CreateExternalEvent(IdentityAddress.Parse(recipient), ExternalEventType.MessageReceived, payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while processing an domain event.");
                throw;
            }
        }
    }
}

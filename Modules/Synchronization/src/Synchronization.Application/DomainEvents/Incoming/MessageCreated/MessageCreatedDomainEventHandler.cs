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
        try
        {
            await CreateMessageReceivedExternalEvents(domainEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }

    private async Task CreateMessageReceivedExternalEvents(MessageCreatedDomainEvent domainEvent)
    {
        foreach (var recipient in domainEvent.Recipients)
        {
            var payload = new MessageReceivedExternalEvent.EventPayload { Id = domainEvent.Id };

            var externalEvent = new MessageReceivedExternalEvent(IdentityAddress.Parse(recipient), payload);

            await _dbContext.CreateExternalEvent(externalEvent);
        }
    }
}

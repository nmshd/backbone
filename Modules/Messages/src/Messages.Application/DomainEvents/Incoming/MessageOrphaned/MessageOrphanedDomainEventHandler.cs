using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.DomainEvents.Incoming;
using Backbone.Modules.Messages.Domain.Ids;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Messages.Application.DomainEvents.Incoming.MessageOrphaned;

public class MessageOrphanedDomainEventHandler : IDomainEventHandler<MessageOrphanedDomainEvent>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly ILogger<MessageOrphanedDomainEventHandler> _logger;

    public MessageOrphanedDomainEventHandler(IMessagesRepository messagesRepository, ILogger<MessageOrphanedDomainEventHandler> logger)
    {
        _messagesRepository = messagesRepository;
        _logger = logger;
    }

    public async Task Handle(MessageOrphanedDomainEvent @event)
    {
        var message = await _messagesRepository.Find(MessageId.Parse(@event.MessageId), @event.IdentityAddress, CancellationToken.None);

        await _messagesRepository.Delete(message);
        _logger.LogTrace("Message with id {Message.Id} deleted because all participants where deleted.", @event.MessageId);
    }
}

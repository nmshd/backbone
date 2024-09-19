using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using MessageId = Backbone.Modules.Messages.Domain.Ids.MessageId;

namespace Backbone.Modules.Messages.Application.DomainEvents.Incoming.MessageOrphaned;

public class MessageOrphanedDomainEventHandler : IDomainEventHandler<MessageOrphanedDomainEvent>
{
    private readonly IMessagesRepository _messagesRepository;

    public MessageOrphanedDomainEventHandler(IMessagesRepository messagesRepository)
    {
        _messagesRepository = messagesRepository;
    }

    public async Task Handle(MessageOrphanedDomainEvent @event)
    {
        var message = await _messagesRepository.Find(MessageId.Parse(@event.MessageId), @event.CreatedBy, CancellationToken.None);

        await _messagesRepository.Delete(message, CancellationToken.None);
    }
}

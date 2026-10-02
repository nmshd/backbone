using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Messages.Abstractions.Persistence;
using Backbone.Modules.Messages.Contracts.DomainEvents;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Module.Features.DomainEvents.MessageOrphaned;

public class Handler(IMessagesRepository messagesRepository) : IDomainEventHandler<MessageOrphanedDomainEvent>
{
    public async Task Handle(MessageOrphanedDomainEvent @event)
    {
        await messagesRepository.Delete(MessageId.Parse(@event.MessageId), CancellationToken.None);
    }
}

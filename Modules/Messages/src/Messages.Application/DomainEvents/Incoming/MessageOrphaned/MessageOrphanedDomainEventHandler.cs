using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.DomainEvents.Incoming;
using Backbone.Modules.Messages.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.DomainEvents.Incoming.MessageOrphaned;

public class MessageOrphanedDomainEventHandler : IDomainEventHandler<MessageOrphanedDomainEvent>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly ApplicationOptions _applicationOptions;

    public MessageOrphanedDomainEventHandler(IMessagesRepository messagesRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _messagesRepository = messagesRepository;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(MessageOrphanedDomainEvent @event)
    {
        await _messagesRepository.Delete(Message.IsMessageOrphaned(_applicationOptions.DidDomainName), CancellationToken.None);
    }
}

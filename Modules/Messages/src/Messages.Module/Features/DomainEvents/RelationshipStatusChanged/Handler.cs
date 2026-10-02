using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Abstractions.Persistence;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Module.Features.DomainEvents.RelationshipStatusChanged;

public class Handler : IDomainEventHandler<RelationshipStatusChangedDomainEvent>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly ILogger<Handler> _logger;
    private readonly ApplicationConfiguration _configuration;

    public Handler(IMessagesRepository messagesRepository, IOptions<ApplicationConfiguration> options, ILogger<Handler> logger)
    {
        _messagesRepository = messagesRepository;
        _logger = logger;
        _configuration = options.Value;
    }

    public async Task Handle(RelationshipStatusChangedDomainEvent @event)
    {
        if (@event.NewStatus != RelationshipStatus.ReadyForDeletion.ToString() && @event.NewStatus != RelationshipStatus.DeletionProposed.ToString())
        {
            _logger.LogTrace("Relationship status changed to {newStatus}. No Message decomposition required.", @event.NewStatus);
            return;
        }

        var anonymizedAddress = IdentityAddress.GetAnonymized(_configuration.DidDomainName);
        var messages = (await _messagesRepository.ListWithoutContent(
            Message.WasExchangedBetween(@event.Initiator, @event.Peer), CancellationToken.None)).ToList();

        foreach (var message in messages)
        {
            if (@event.WasDueToIdentityDeletion)
                message.AnonymizeParticipant(@event.Initiator, anonymizedAddress);
            else
                message.DecomposeFor(@event.Initiator, @event.Peer, anonymizedAddress);
        }

        await _messagesRepository.Update(messages);
    }
}

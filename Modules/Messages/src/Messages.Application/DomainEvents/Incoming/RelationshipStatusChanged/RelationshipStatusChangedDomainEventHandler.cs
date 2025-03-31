using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.DomainEvents.Incoming;
using Backbone.Modules.Messages.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.DomainEvents.Incoming.RelationshipStatusChanged;

public class RelationshipStatusChangedDomainEventHandler : IDomainEventHandler<RelationshipStatusChangedDomainEvent>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly ILogger<RelationshipStatusChangedDomainEventHandler> _logger;
    private readonly ApplicationConfiguration _applicationConfiguration;

    public RelationshipStatusChangedDomainEventHandler(IMessagesRepository messagesRepository, IOptions<ApplicationConfiguration> applicationOptions,
        ILogger<RelationshipStatusChangedDomainEventHandler> logger)
    {
        _messagesRepository = messagesRepository;
        _logger = logger;
        _applicationConfiguration = applicationOptions.Value;
    }

    public async Task Handle(RelationshipStatusChangedDomainEvent @event)
    {
        if (@event.NewStatus != RelationshipStatus.ReadyForDeletion.ToString() && @event.NewStatus != RelationshipStatus.DeletionProposed.ToString())
        {
            _logger.LogTrace("Relationship status changed to {newStatus}. No Message decomposition required.", @event.NewStatus);
            return;
        }

        var anonymizedIdentityAddress = IdentityAddress.GetAnonymized(_applicationConfiguration.DidDomainName);
        var messagesExchangedBetweenRelationshipParticipants = (await _messagesRepository.Find(Message.WasExchangedBetween(@event.Initiator, @event.Peer), CancellationToken.None)).ToList();

        foreach (var message in messagesExchangedBetweenRelationshipParticipants)
        {
            message.DecomposeFor(@event.Initiator, @event.Peer, anonymizedIdentityAddress);
        }

        await _messagesRepository.Update(messagesExchangedBetweenRelationshipParticipants);
    }
}

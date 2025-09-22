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
        var messagesExchangedBetweenRelationshipParticipants =
            (await _messagesRepository.ListWithoutContent(Message.WasExchangedBetween(@event.Initiator, @event.Peer), CancellationToken.None)).ToList();

        foreach (var message in messagesExchangedBetweenRelationshipParticipants)
        {
            // If the status was changed due to identity deletion, we are forced to anonymize the address of the identity that deleted itself.
            // If it was a manual status change though, we only want to mark the identity as decomposed on this message. Only after both
            // participants have decomposed the relationship the message will be anonymized/deleted.
            if (@event.WasDueToIdentityDeletion)
                message.AnonymizeParticipant(@event.Initiator, anonymizedIdentityAddress);
            else
                message.DecomposeFor(@event.Initiator, @event.Peer, anonymizedIdentityAddress);
        }

        await _messagesRepository.Update(messagesExchangedBetweenRelationshipParticipants);
    }
}

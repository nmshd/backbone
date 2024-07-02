using System.Linq.Expressions;
using System.Text;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.DomainEvents.Incoming;
using Backbone.Modules.Messages.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.DomainEvents.Incoming.RelationshipStatusChanged;

public class RelationshipStatusChangedDomainEventHandler : IDomainEventHandler<RelationshipStatusChangedDomainEvent>
{
    private const string DELETED_IDENTITY_STRING = "deleted identity";
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IMessagesRepository _messagesRepository;
    private readonly ApplicationOptions _applicationOptions;

    public RelationshipStatusChangedDomainEventHandler(IRelationshipsRepository relationshipsRepository, IMessagesRepository messagesRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _relationshipsRepository = relationshipsRepository;
        _messagesRepository = messagesRepository;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(RelationshipStatusChangedDomainEvent @event)
    {
        if (@event.Status != RelationshipStatus.ReadyForDeletion.ToString())
            return;

        var messagesSentFromInitiator = (await _messagesRepository.FindMessagesFromSenderToRecipient(@event.Initiator, @event.Peer, CancellationToken.None)).ToList();
        await AnonymizeParticipantInMessages(messagesSentFromInitiator, @event.Peer);
        foreach (var message in messagesSentFromInitiator)
        {
            var activeRelationshipsCount = await _relationshipsRepository.CountActiveRelationships(
                HasActiveRelationshipBetweenAMessageRecipientAndParticipant(message.Recipients.ToList(), @event.Initiator),
                CancellationToken.None);

            if (activeRelationshipsCount == 0)
                await AnonymizeParticipantInMessages(messagesSentFromInitiator, @event.Initiator);
        }


        var messagesSentFromPeer = (await _messagesRepository.FindMessagesFromSenderToRecipient(@event.Peer, @event.Initiator, CancellationToken.None)).ToList();
        await AnonymizeParticipantInMessages(messagesSentFromPeer, @event.Initiator);
        foreach (var message in messagesSentFromPeer)
        {
            var activeRelationshipsCount = await _relationshipsRepository.CountActiveRelationships(
                HasActiveRelationshipBetweenAMessageRecipientAndParticipant(message.Recipients.ToList(), @event.Peer),
                CancellationToken.None);

            if (activeRelationshipsCount == 0)
                await AnonymizeParticipantInMessages(messagesSentFromInitiator, @event.Peer);
        }
    }

    private async Task AnonymizeParticipantInMessages(List<Message> messages, string participantAddress)
    {
        var newIdentityAddress = IdentityAddress.Create(Encoding.Unicode.GetBytes(DELETED_IDENTITY_STRING), _applicationOptions.DidDomainName);
        foreach (var message in messages)
        {
            message.ReplaceIdentityAddress(participantAddress, newIdentityAddress);
        }

        await _messagesRepository.Update(messages);
    }

    public static Expression<Func<Relationship, bool>> HasActiveRelationshipBetweenAMessageRecipientAndParticipant(List<RecipientInformation> recipients, string participantAddress)
    {
        return r => (recipients.Any(rec => rec.Address == r.From) && r.To == participantAddress && r.Status == RelationshipStatus.Active)
                    || (recipients.Any(rec => rec.Address == r.To) && r.From == participantAddress && r.Status == RelationshipStatus.Active);
    }
}

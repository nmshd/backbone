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
    private readonly IMessagesRepository _messagesRepository;
    private readonly ApplicationOptions _applicationOptions;

    public RelationshipStatusChangedDomainEventHandler(IMessagesRepository messagesRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _messagesRepository = messagesRepository;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(RelationshipStatusChangedDomainEvent @event)
    {
        if (@event.NewStatus != RelationshipStatus.ReadyForDeletion.ToString())
            return;

        var anonymizedIdentityAddress = IdentityAddress.Create(Encoding.Unicode.GetBytes(DELETED_IDENTITY_STRING), _applicationOptions.DidDomainName);
        var messagesExchangedBetweenRelationshipParticipants = (await _messagesRepository.Find(Message.WasExchangedBetween(@event.Initiator, @event.Peer), CancellationToken.None)).ToList();
        foreach (var message in messagesExchangedBetweenRelationshipParticipants)
        {
            message.SanitizeAfterRelationshipDeleted(@event.Initiator, @event.Peer, anonymizedIdentityAddress);
        }

        await _messagesRepository.Update(messagesExchangedBetweenRelationshipParticipants);
    }
}

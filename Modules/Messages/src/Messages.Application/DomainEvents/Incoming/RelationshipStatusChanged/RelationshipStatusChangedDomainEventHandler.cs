﻿using System.Text;
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
    private const string DELETED_IDENTITY_STRING = "deleted identity";
    private readonly IMessagesRepository _messagesRepository;
    private readonly ILogger<RelationshipStatusChangedDomainEventHandler> _logger;
    private readonly ApplicationOptions _applicationOptions;

    public RelationshipStatusChangedDomainEventHandler(IMessagesRepository messagesRepository, IOptions<ApplicationOptions> applicationOptions,
        ILogger<RelationshipStatusChangedDomainEventHandler> logger)
    {
        _messagesRepository = messagesRepository;
        _logger = logger;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(RelationshipStatusChangedDomainEvent @event)
    {
        if (@event.NewStatus != RelationshipStatus.ReadyForDeletion.ToString())
        {
            _logger.LogTrace("Relationship status changed to {newStatus}. No Message anonymization required.", @event.NewStatus);
            return;
        }

        var anonymizedIdentityAddress = IdentityAddress.Create(Encoding.Unicode.GetBytes(DELETED_IDENTITY_STRING), _applicationOptions.DidDomainName);
        var messagesExchangedBetweenRelationshipParticipants = (await _messagesRepository.Find(Message.WasExchangedBetween(@event.Initiator, @event.Peer), CancellationToken.None)).ToList();
        foreach (var message in messagesExchangedBetweenRelationshipParticipants)
        {
            message.SanitizeAfterRelationshipDeleted(@event.Initiator, @event.Peer, anonymizedIdentityAddress);
        }

        await _messagesRepository.Update(messagesExchangedBetweenRelationshipParticipants);
    }
}

internal static partial class RelationshipStatusChangedLogs
{
    [LoggerMessage(
        EventName = "Messages.RelationshipStatusChangedDomainEventHandler.RelationshipStatusChanged",
        Level = LogLevel.Debug,
        Message = "Relationship status changed to {newStatus}. No Message anonymization required.")]
    public static partial void RelationshipStatusChanged(this ILogger logger, string newStatus);
}

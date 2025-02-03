using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Domain.Entities.Relationships;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.MessageCreated;

public class MessageCreatedDomainEventHandler : IDomainEventHandler<MessageCreatedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly ILogger<MessageCreatedDomainEventHandler> _logger;

    public MessageCreatedDomainEventHandler(ISynchronizationDbContext dbContext, IRelationshipsRepository relationshipsRepository, ILogger<MessageCreatedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _relationshipsRepository = relationshipsRepository;
        _logger = logger;
    }

    public async Task Handle(MessageCreatedDomainEvent @event)
    {
        try
        {
            await CreateMessageReceivedExternalEvents(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }

    private async Task CreateMessageReceivedExternalEvents(MessageCreatedDomainEvent @event)
    {
        var relationships = await _relationshipsRepository.GetRelationships(@event.Recipients.Select(r => RelationshipId.Parse(r.RelationshipId)), CancellationToken.None);

        foreach (var recipient in @event.Recipients)
        {
            var relationship = GetRelationshipBetween(@event.CreatedBy, recipient.Address, relationships);
            await CreateExternalEventForRecipient(@event, recipient.Address, relationship);
        }
    }

    private async Task CreateExternalEventForRecipient(MessageCreatedDomainEvent @event, string recipient, Relationship relationship)
    {
        var payload = new MessageReceivedExternalEvent.EventPayload { Id = @event.Id };

        var externalEvent = new MessageReceivedExternalEvent(IdentityAddress.Parse(recipient), payload, relationship.Id);

        if (relationship.Status is RelationshipStatus.Pending or RelationshipStatus.Terminated)
            externalEvent.BlockDelivery();

        await _dbContext.CreateExternalEvent(externalEvent);
    }

    private static Relationship GetRelationshipBetween(string identity1, string identity2, List<Relationship> relationships)
    {
        var relationship = relationships.Single(r =>
        {
            var createdByAddress = IdentityAddress.ParseUnsafe(identity1);
            var recipientAddress = IdentityAddress.ParseUnsafe(identity2);

            return r.IsBetween(createdByAddress, recipientAddress);
        });
        return relationship;
    }
}

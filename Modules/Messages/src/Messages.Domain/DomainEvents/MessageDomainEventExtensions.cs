using Backbone.Modules.Messages.Contracts.DomainEvents;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Domain.DomainEvents;

public static class MessageCreatedDomainEventExtensions
{
    extension(MessageCreatedDomainEvent)
    {
        public static MessageCreatedDomainEvent Create(Message message)
        {
            return new MessageCreatedDomainEvent
            {
                DomainEventId = $"{message.Id}/Created",
                Id = message.Id.Value,
                Recipients = message.Recipients.Select(r => new MessageCreatedDomainEvent.Recipient
                {
                    Address = r.Address.Value,
                    RelationshipId = r.RelationshipId.Value
                }),
                CreatedBy = message.CreatedBy.Value
            };
        }
    }
}

public static class MessageOrphanedDomainEventExtensions
{
    extension(MessageOrphanedDomainEvent)
    {
        public static MessageOrphanedDomainEvent Create(Message message)
        {
            return new MessageOrphanedDomainEvent
            {
                DomainEventId = $"{message.Id}/MessageOrphaned",
                MessageId = message.Id
            };
        }
    }
}

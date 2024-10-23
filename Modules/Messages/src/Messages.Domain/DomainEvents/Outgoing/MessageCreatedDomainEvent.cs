using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;

public class MessageCreatedDomainEvent : DomainEvent
{
    public MessageCreatedDomainEvent(Message message) : base($"{message.Id}/Created")
    {
        Id = message.Id.Value;
        Recipients = message.Recipients.Select(r => new Recipient
        {
            Address = r.Address.Value,
            RelationshipId = r.RelationshipId.Value
        });
        CreatedBy = message.CreatedBy.Value;
    }

    public string Id { get; }
    public IEnumerable<Recipient> Recipients { get; }
    public string CreatedBy { get; }

    public class Recipient
    {
        public required string Address { get; set; }
        public required string RelationshipId { get; set; }
    }
}

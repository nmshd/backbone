using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Messages.Contracts.DomainEvents;

public class MessageCreatedDomainEvent : DomainEvent
{
    public required string Id { get; set; }
    public required IEnumerable<Recipient> Recipients { get; set; }
    public required string CreatedBy { get; set; }

    public class Recipient
    {
        public required string Address { get; set; }
        public required string RelationshipId { get; set; }
    }
}

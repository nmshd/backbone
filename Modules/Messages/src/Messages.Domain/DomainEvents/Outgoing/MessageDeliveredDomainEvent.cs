using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;

public class MessageDeliveredDomainEvent : DomainEvent
{
    public MessageDeliveredDomainEvent(Message message, IdentityAddress deliveredTo) : base($"{message.Id}/DeliveredTo/{deliveredTo}")
    {
        MessageId = message.Id;
        Sender = message.CreatedBy;
        DeliveredTo = deliveredTo;
    }

    public string MessageId { get; }
    public string Sender { get; }
    public string DeliveredTo { get; }
}

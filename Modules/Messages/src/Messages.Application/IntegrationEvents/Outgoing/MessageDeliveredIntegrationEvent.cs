using Backbone.Modules.Messages.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Messages.Application.IntegrationEvents.Outgoing;

public class MessageDeliveredIntegrationEvent : IntegrationEvent
{
    public MessageDeliveredIntegrationEvent(Message message, IdentityAddress deliveredTo) : base($"{message.Id}/DeliveredTo/{deliveredTo}")
    {
        MessageId = message.Id;
        Sender = message.CreatedBy;
        DeliveredTo = deliveredTo;
    }

    public string MessageId { get; }
    public string Sender { get; }
    public string DeliveredTo { get; }
}

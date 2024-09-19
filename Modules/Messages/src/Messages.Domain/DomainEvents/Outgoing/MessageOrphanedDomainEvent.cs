using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Domain.DomainEvents.Outgoing
{
    public class MessageOrphanedDomainEvent : DomainEvent
    {
        public MessageOrphanedDomainEvent(MessageId messageId, IdentityAddress createdBy) : base($"{messageId}/MessageOrphaned")
        {
            MessageId = messageId;
            CreatedBy = createdBy;
        }

        public string MessageId { get; }
        public string CreatedBy { get; }
    }
}

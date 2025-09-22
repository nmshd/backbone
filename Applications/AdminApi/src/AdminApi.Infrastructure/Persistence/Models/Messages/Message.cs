// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Messages;

public class Message
{
    public required string Id { get; init; }
    public required string CreatedBy { get; init; }
    public required string CreatedByDevice { get; init; }
    public virtual required List<MessageRecipient> Recipients { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required byte[] Body { get; init; }
    public virtual required IEnumerable<MessageAttachment> Attachments { get; init; }
}

public class MessageAttachment
{
    public required string Id { get; init; }
    public required string MessageId { get; init; }
}

public class MessageRecipient
{
    public required int Id { get; init; }

    public required string MessageId { get; init; }
    public required string RelationshipId { get; init; }
    public required string Address { get; init; }
    public required DateTime? ReceivedAt { get; init; }
}

namespace Backbone.Modules.Quotas.Domain.Aggregates.Messages;
public class Message : ICreatedAt
{
    public Message(string id, string createdBy, DateTime createdAt)
    {
        Id = id;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Message() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public string Id { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

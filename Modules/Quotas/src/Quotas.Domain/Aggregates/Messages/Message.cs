namespace Backbone.Modules.Quotas.Domain.Aggregates.Messages;

public class Message : ICreatedAt
{
    public required string Id { get; set; }
    public required string CreatedBy { get; set; }
    public required DateTime CreatedAt { get; set; }
}

namespace Backbone.Modules.Quotas.Domain.Aggregates.Messages;
public class Message : ICreatedAt
{
    public string? Id { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

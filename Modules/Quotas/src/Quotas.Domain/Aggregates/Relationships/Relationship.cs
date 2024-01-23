namespace Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
public class Relationship : ICreatedAt
{
    public string? Id { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public RelationshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
public enum RelationshipStatus
{
    Pending = 10,
    Active = 20,
    Rejected = 30,
    Revoked = 40,
    Terminating = 50,
    Terminated = 60
}

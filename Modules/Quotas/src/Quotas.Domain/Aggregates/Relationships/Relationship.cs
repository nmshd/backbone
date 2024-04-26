namespace Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
public class Relationship : ICreatedAt
{
    public required string Id { get; set; }
    public required string From { get; set; }
    public required string To { get; set; }
    public required RelationshipStatus Status { get; set; }
    public required List<RelationshipAuditLogEntry> AuditLog { get; set; }
    public required DateTime CreatedAt { get; set; }
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

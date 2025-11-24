namespace Backbone.Modules.Quotas.Domain.Aggregates.Relationships;

public class Relationship : ICreatedAt
{
    public required string Id { get; set; }
    public required string From { get; set; }
    public required string To { get; set; }
    public required RelationshipStatus Status { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required bool FromHasDecomposed { get; set; }
    public required bool ToHasDecomposed { get; set; }
}
public enum RelationshipStatus
{
    Pending = 10,
    Active = 20,
    Rejected = 30,
    Revoked = 40,
    Terminated = 50,
    DeletionProposed = 60
}

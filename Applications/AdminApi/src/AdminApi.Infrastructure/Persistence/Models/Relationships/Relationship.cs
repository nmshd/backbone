namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Relationships;

public class Relationship
{
    public string Id { get; set; } = null!;
    public string? RelationshipTemplateId { get; set; }
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public RelationshipStatus Status { get; set; }
    public bool FromHasDecomposed { get; set; }
    public bool ToHasDecomposed { get; set; }
    public RelationshipTemplate? RelationshipTemplate { get; set; }
    public IList<RelationshipAuditLogItem> AuditLog { get; set; } = null!;
}

public enum RelationshipStatus
{
    Pending = 10,
    Active = 20,
    Rejected = 30,
    Revoked = 40,
    Terminated = 50,
    DeletionProposed = 60,
    ReadyForDeletion = 70
}

public class RelationshipAuditLogItem
{
    public string Id { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string CreatedByDevice { get; set; } = null!;
}

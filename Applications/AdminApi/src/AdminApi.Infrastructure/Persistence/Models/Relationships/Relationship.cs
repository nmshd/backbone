// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Relationships;

public class Relationship
{
    public required string Id { get; init; }
    public required string? RelationshipTemplateId { get; init; }
    public required string From { get; init; }
    public required string To { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required RelationshipStatus Status { get; init; }
    public required bool FromHasDecomposed { get; init; }
    public required bool ToHasDecomposed { get; init; }
    public virtual required RelationshipTemplate? RelationshipTemplate { get; init; }
    public virtual required IList<RelationshipAuditLogItem> AuditLog { get; init; }
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
    public required string Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string CreatedByDevice { get; init; }
}

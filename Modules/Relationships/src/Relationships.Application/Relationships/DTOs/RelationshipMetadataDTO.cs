using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipMetadataDTO
{
    public RelationshipMetadataDTO(Relationship relationship)
    {
        Id = relationship.Id;
        RelationshipTemplateId = relationship.RelationshipTemplateId?.Value;
        From = relationship.From;
        To = relationship.To;
        CreatedAt = relationship.CreatedAt;
        Status = relationship.Status;
        AuditLog = relationship.AuditLog.Select(a => new RelationshipAuditLogEntryDTO(a)).ToList();
    }

    public string Id { get; set; }
    public string? RelationshipTemplateId { get; set; }

    public string From { get; set; }
    public string To { get; set; }

    public DateTime CreatedAt { get; set; }

    public RelationshipStatus Status { get; set; }

    public List<RelationshipAuditLogEntryDTO> AuditLog { get; set; }
}

public static class RelationshipStatusExtensions
{
    public static string ToDtoString(this RelationshipStatus status)
    {
        return status.ToDtoStringInternal();
    }

    public static string? ToDtoString(this RelationshipStatus? status)
    {
        return status?.ToDtoStringInternal();
    }

    private static string ToDtoStringInternal(this RelationshipStatus status)
    {
        return status switch
        {
            RelationshipStatus.Pending => "Pending",
            RelationshipStatus.Active => "Active",
            RelationshipStatus.Rejected => "Rejected",
            RelationshipStatus.Revoked => "Revoked",
            RelationshipStatus.Terminated => "Terminated",
            RelationshipStatus.DeletionProposed => "DeletionProposed",
            RelationshipStatus.ReadyForDeletion => "ReadyForDeletion",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}

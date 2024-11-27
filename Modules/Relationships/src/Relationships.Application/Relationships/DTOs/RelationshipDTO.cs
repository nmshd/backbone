using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipDTO
{
    public RelationshipDTO(Relationship relationship)
    {
        Id = relationship.Id;
        RelationshipTemplateId = relationship.RelationshipTemplateId?.Value;
        From = relationship.From;
        To = relationship.To;
        CreatedAt = relationship.CreatedAt;
        Status = relationship.Status;
        CreationContent = relationship.CreationContent;
        CreationResponseContent = relationship.CreationResponseContent;
        AuditLog = relationship.AuditLog.Select(a => new RelationshipAuditLogEntryDTO(a)).ToList();
    }

    public string Id { get; set; }
    public string? RelationshipTemplateId { get; set; }

    public string From { get; set; }
    public string To { get; set; }

    public byte[]? CreationContent { get; set; }
    public byte[]? CreationResponseContent { get; set; }

    public DateTime CreatedAt { get; set; }

    public RelationshipStatus Status { get; set; }

    public List<RelationshipAuditLogEntryDTO> AuditLog { get; set; }
}

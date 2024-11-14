using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipAuditLogEntryDTO
{
    public RelationshipAuditLogEntryDTO(RelationshipAuditLogEntry entry)
    {
        CreatedAt = entry.CreatedAt;
        CreatedBy = entry.CreatedBy;
        CreatedByDevice = entry.CreatedByDevice?.Value;
        Reason = entry.Reason.ToString();
        OldStatus = entry.OldStatus.ToDtoString();
        NewStatus = entry.NewStatus.ToDtoString();
    }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string? CreatedByDevice { get; set; }
    public string Reason { get; set; }

    public string? OldStatus { get; set; }
    public string NewStatus { get; set; }
}

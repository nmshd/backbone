namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;

public class RelationshipAuditLogEntry
{
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }
    public required string Reason { get; set; }
    public required string? OldStatus { get; set; }
    public required string NewStatus { get; set; }
}

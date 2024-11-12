namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;

public class RelationshipMetadata
{
    public required string Id { get; set; }
    public string? RelationshipTemplateId { get; set; }

    public required string From { get; set; }
    public required string To { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required string Status { get; set; }

    public required List<RelationshipAuditLogEntry> AuditLog { get; set; }
}

namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;

public class RelationshipMetadataDTO
{
    public required string Id { get; set; }
    public required string RelationshipTemplateId { get; set; }

    public required string From { get; set; }
    public required string To { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required string Status { get; set; }

    public required List<RelationshipAuditLogEntryDTO> AuditLog { get; set; }
}

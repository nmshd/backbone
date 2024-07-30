namespace Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types;

public class RelationshipTemplate
{
    public required string Id { get; set; }

    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public byte[]? Content { get; set; }

    public required DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

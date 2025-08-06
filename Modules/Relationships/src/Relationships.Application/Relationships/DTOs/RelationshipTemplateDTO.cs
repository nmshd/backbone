using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipTemplateDTO
{
    public RelationshipTemplateDTO(RelationshipTemplate relationshipTemplate)
    {
        Id = relationshipTemplate.Id;
        CreatedBy = relationshipTemplate.CreatedBy;
        CreatedByDevice = relationshipTemplate.CreatedByDevice;
        MaxNumberOfAllocations = relationshipTemplate.MaxNumberOfAllocations;
        ExpiresAt = relationshipTemplate.ExpiresAt;
        Content = relationshipTemplate.Details.Content;
        CreatedAt = relationshipTemplate.CreatedAt;
        ForIdentity = relationshipTemplate.ForIdentity?.Value;
    }

    public string Id { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedByDevice { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public byte[]? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ForIdentity { get; set; }
}

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Relationships;

public class RelationshipTemplate
{
    public string Id { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public IEnumerable<RelationshipTemplateAllocation> Allocations { get; set; } = null!;
}

public class RelationshipTemplateAllocation
{
    public string Id { get; set; } = null!;
    public DateTime? AllocatedAt { get; set; }
}

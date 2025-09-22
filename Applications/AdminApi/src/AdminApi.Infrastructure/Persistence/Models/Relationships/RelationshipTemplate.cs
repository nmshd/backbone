// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Relationships;

public class RelationshipTemplate
{
    public required string Id { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTime CreatedAt { get; init; }
    public virtual required IEnumerable<RelationshipTemplateAllocation> Allocations { get; init; }
}

public class RelationshipTemplateAllocation
{
    public required string Id { get; init; }
    public required DateTime? AllocatedAt { get; init; }
}

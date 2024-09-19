namespace Backbone.Modules.Quotas.Domain.Aggregates.Relationships;

public class RelationshipTemplate : ICreatedAt
{
    public required string CreatedBy { get; set; }
    public required DateTime CreatedAt { get; set; }
}

namespace Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
public class RelationshipTemplate : ICreatedAt
{
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

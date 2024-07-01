namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class Device : ICreatedAt
{
    public required string Id { get; set; }
    public required string IdentityAddress { get; set; }
    public required DateTime CreatedAt { get; set; }
}

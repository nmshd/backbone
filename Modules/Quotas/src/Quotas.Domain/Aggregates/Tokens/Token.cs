namespace Backbone.Modules.Quotas.Domain.Aggregates.Tokens;

public class Token : ICreatedAt
{
    public required string? CreatedBy { get; set; }
    public required DateTime CreatedAt { get; set; }
}

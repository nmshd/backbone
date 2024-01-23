namespace Backbone.Modules.Quotas.Domain.Aggregates.Tokens;
public class Token : ICreatedAt
{
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

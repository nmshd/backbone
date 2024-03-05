namespace Backbone.AdminApi.Sdk.Endpoints.Relationships.Types;

public class Relationship
{
    public required string Peer { get; set; }
    public required string RequestedBy { get; set; }
    public required string TemplateId { get; set; }
    public required string Status { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? AnsweredAt { get; set; }
    public required string CreatedByDevice { get; set; }
    public string? AnsweredByDevice { get; set; }
}

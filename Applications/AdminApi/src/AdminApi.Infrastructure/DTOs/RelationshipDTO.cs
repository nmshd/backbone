namespace Backbone.AdminApi.Infrastructure.DTOs;

public class RelationshipDTO
{
    public string Peer { get; set; } = null!;
    public string RequestedBy { get; set; } = null!;
    public string? TemplateId { get; set; }
    public RelationshipStatus Status { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? AnsweredAt { get; set; }
    public string CreatedByDevice { get; set; } = null!;
    public string? AnsweredByDevice { get; set; }
}

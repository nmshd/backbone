using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.AdminUi.Infrastructure.DTOs;
public class RelationshipDTO
{
    public string Peer { get; set; }
    public string RequestedBy { get; set; }
    public string TemplateId { get; set; }
    public RelationshipStatus Status { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? AnsweredAt { get; set; }
    public string CreatedByDevice { get; set; }
    public string? AnsweredByDevice { get; set; }

    public RelationshipDTO(IdentityAddress pointOfView, RelationshipOverview relationshipOverview)
    {
        Peer = relationshipOverview.To == pointOfView ? relationshipOverview.From : relationshipOverview.To;
        RequestedBy = relationshipOverview.To == pointOfView ? "Peer" : "Self";
        TemplateId = relationshipOverview.RelationshipTemplateId;
        Status = relationshipOverview.Status;
        CreationDate = relationshipOverview.CreatedAt;
        AnsweredAt = relationshipOverview.AnsweredAt;
        CreatedByDevice = relationshipOverview.CreatedByDevice;
        AnsweredByDevice = relationshipOverview.AnsweredByDevice;
    }
}

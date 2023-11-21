namespace Backbone.AdminUi.Infrastructure.DTOs;
public class RelationshipOverview
{
    public string From { get; set; }
    public string To { get; set; }
    public string RelationshipTemplateId { get; set; }
    public RelationshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AnsweredAt { get; set; }
    public string CreatedByDevice { get; set; }
    public string? AnsweredByDevice { get; set; }
}

public enum RelationshipStatus
{
    Pending = 10,
    Active = 20,
    Rejected = 30,
    Revoked = 40,
    Terminating = 50,
    Terminated = 60
}

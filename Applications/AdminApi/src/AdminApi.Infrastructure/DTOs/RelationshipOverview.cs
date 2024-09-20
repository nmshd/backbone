namespace Backbone.AdminApi.Infrastructure.DTOs;
public class RelationshipOverview
{
    public required string From { get; set; }
    public required string To { get; set; }
    public required string RelationshipTemplateId { get; set; }
    public required RelationshipStatus Status { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? AnsweredAt { get; set; }
    public required string CreatedByDevice { get; set; }
    public required string? AnsweredByDevice { get; set; }
}

public enum RelationshipStatus
{
    Pending = 10,
    Active = 20,
    Rejected = 30,
    Revoked = 40,
    Terminated = 50,
    DeletionProposed = 60,
    ReadyForDeletion = 70
}

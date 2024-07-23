using Backbone.Modules.Challenges.Domain.Entities;

namespace Backbone.Modules.Challenges.Application.Challenges.DTOs;

public class ChallengeDTO
{
    public ChallengeDTO(Challenge challenge)
    {
        Id = challenge.Id.Value;
        ExpiresAt = challenge.ExpiresAt;
        CreatedBy = challenge.CreatedBy?.Value;
        CreatedByDevice = challenge.CreatedByDevice?.Value;
    }

    public string Id { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedByDevice { get; set; }
}
